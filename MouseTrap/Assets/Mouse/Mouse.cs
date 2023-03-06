using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    /* PUBLIC VARS */
    //*************************************************************************
    public GameObject mouseHex;
    public GameObject tgtHex;
    public int chosenHex;
    public List<Node> shortestPath = new List<Node>();

    public const float expandedColliderRadius = 8.175106f;
    //*************************************************************************

    /* PRIVATE VARS */
    //*************************************************************************
    private Manager manager;
    private List<GameObject> potentialTargetList = new List<GameObject>();
    private bool retarget = false;
    private int maxSearch = 6;
    private int turnCount = 0;
    //*************************************************************************


    // Start is called before the first frame update
    void Start()
    {
        // Attach manager object
        manager = GetComponentInParent<Manager>();

        // Establish initial target 
        potentialTargetList = manager.mapHexes.FindAll(mapHex =>
            mapHex.GetComponent<MapHex>().isEdge == true &&
            !mapHex.GetComponent<MapHex>().isClicked);
        chosenHex = Random.Range(0, potentialTargetList.Count);
        tgtHex = potentialTargetList[chosenHex];
    }

    // Update is called once per frame
    void Update()
    {
        // Disallow mouse movement unless its the mouse's turn and no one won
        if (!manager.userTurn)
        {
            CheckWinLoss();
            if (!manager.userWin && !manager.mouseWin)
            {
                TargetLogic(); 
                ComputeHexGraph();
                MoveMouse();
            }

            manager.userTurn = true;
            turnCount++;
        }
    }

    // reassign to nearby target if the initial target is inaccessible
    void TargetLogic()
    {
        if (retarget || tgtHex.GetComponent<MapHex>().isClicked)
        {
            while (tgtHex.GetComponent<MapHex>().isClicked
                || tgtHex.GetComponent<MapHex>().inaccessible)
            {
                List<MapHex> adjHex = manager.GetAdjacentHexes(tgtHex,
                    MapHex.nominalColliderRadius,
                    MapHex.expandedColliderRadius, true);
                var newTgtHex = adjHex.FirstOrDefault(hex =>
                    hex.isEdge == true && !hex.isClicked && !hex.inaccessible);
                if (newTgtHex != null)
                {
                    tgtHex = newTgtHex.gameObject;
                }
                else
                {
                    CheckWinLoss();
                    if (manager.userWin)
                        break;
                    tgtHex = adjHex.FirstOrDefault(hex =>
                        hex.isEdge == true && hex.isClicked).gameObject;   
                }
            }
            retarget = false;
        }
    }

    // Always check for a closer win so the mouse doesn't look stupid
    // Accomplish this by using the list of potential targets (edge pieces)
    // and sorting by straight line distance to mouse. If a shorter path is
    // available, take it
    void CheckForCloserTarget()
    {
        double originalShortestLength = shortestPath.Count();
        Node originalNextMove = shortestPath[0].ClonePoint();
        int counter = 0;

        potentialTargetList = potentialTargetList.
            OrderByDescending(tgt => tgt.GetComponent<MapHex>().node.
            StraightLineDistanceTo(mouseHex.GetComponent<MapHex>().node))
            .Reverse().ToList();
        foreach (GameObject potentialTarget in potentialTargetList)
        {
            if (!potentialTarget.GetComponent<MapHex>().isClicked
                && !potentialTarget.GetComponent<MapHex>().inaccessible
                && potentialTarget != tgtHex)
            {
                tgtHex = potentialTarget;
                ComputeHexGraph();
                shortestPath = GetShortestPathAstar();
                counter++;

                if (shortestPath.Count < originalShortestLength &&
                    shortestPath.Count > 0)
                    break;
                if (counter > maxSearch)
                {
                    shortestPath.Clear();
                    shortestPath.Add(originalNextMove);
                    break;
                }
            }
        }
    }

    // Randomly fuck up shortest path by choosing an adjacent hex at random
    void BlunderLogic()
    {
        if(Random.Range(0, 101) > (100-manager.currentLevel.mouseBlunderPercentage))
        {
            List<MapHex> adjHex =
                manager.GetAdjacentHexes(manager.mouse.GetComponent<Mouse>().
                mouseHex, MapHex.nominalColliderRadius,
                MapHex.expandedColliderRadius);
            shortestPath[0] = adjHex[Random.Range(0, adjHex.Count)].node;
        }
    }

    // Move mouse in direction of shortest path. If shortest path undetermined
    // i.e. target is unreachable, then retarget until you get an accessible
    // target. If the target is reachable, scan for a closer potential target
    // to retarget to. Randomly blunder. Make the move
    void MoveMouse()
    {
        shortestPath = GetShortestPathAstar();
        while (shortestPath.Count() == 0)
        {
            retarget = true;
            tgtHex.GetComponent<MapHex>().inaccessible = true;
            TargetLogic();
            if (manager.userWin)
                return;
            ComputeHexGraph();
            shortestPath = GetShortestPathAstar();
        }
        if (shortestPath.Count > 1 && turnCount > 2)
            CheckForCloserTarget();
        BlunderLogic();
        transform.position = new Vector3(shortestPath[0].Point.X,
            shortestPath[0].Point.Y, transform.position.z);
    }

    // Check win/loss condition
    // win condition == mouse reached tgt hex
    // loss condition == mouse has no moves
    void CheckWinLoss()
    {
        if(mouseHex == tgtHex)
        {
            manager.mouseWin = true;
            return;
        }

        List<MapHex> adjHex =
                manager.GetAdjacentHexes(manager.mouse.GetComponent<Mouse>().
                mouseHex, MapHex.nominalColliderRadius,
                MapHex.expandedColliderRadius);
        if(adjHex.All(hex => hex.isClicked))
        {
            manager.userWin = true;
        }

        // Test some targets, if all return no path, assume lost
        potentialTargetList = potentialTargetList.
            OrderByDescending(tgt => tgt.GetComponent<MapHex>().node.
            StraightLineDistanceTo(mouseHex.GetComponent<MapHex>().node))
            .Reverse().ToList();
        foreach(GameObject potentialTarget in potentialTargetList)
        {
            if (!potentialTarget.GetComponent<MapHex>().isClicked
                && !potentialTarget.GetComponent<MapHex>().inaccessible
                && potentialTarget != tgtHex)
            {
                tgtHex = potentialTarget;
                ComputeHexGraph();
                shortestPath = GetShortestPathAstar();

                if (shortestPath.Count != 0)
                    break;
                else if (potentialTarget ==
                    potentialTargetList[potentialTargetList.Count()-1])
                    manager.userWin = true;
            }
        }
    }

    /* BEGIN A* ALGORITHM

    ResetAstarAlgorithm loops through the MapHexes saved in the manager
    object and resets their internal parameters

    ComputeHexGraph resets the nodes data internal to the maphex objects, then
    establishes the connected nodes given that the map
    changes everytime the user clicks a hex

    GetShortestPathAstar calls AstarSeach and BuildShortestPath to actually
    generate the shortest path to the target 

    */
    // ----------------------------------------------------------------------
    void ComputeHexGraph()
    {
        foreach (GameObject mapHex in manager.mapHexes)
        {
            mapHex.GetComponent<MapHex>().node.MinCostToStart = null;
            mapHex.GetComponent<MapHex>().node.Visited = false;
            mapHex.GetComponent<MapHex>().node.StraightLineDistanceToEnd = 0;
            mapHex.GetComponent<MapHex>().node.NearestToStart = null;
            mapHex.GetComponent<MapHex>().node.Connections.Clear();
        }

        List<Node> nodes = new List<Node>();
        foreach (GameObject mapHex in manager.mapHexes)
        {
            if (!mapHex.GetComponent<MapHex>().isClicked)
            {
                List<Edge> edges = new List<Edge>();
                List<MapHex> adjacentHexes = manager.GetAdjacentHexes(mapHex,
                    MapHex.nominalColliderRadius,
                    MapHex.expandedColliderRadius);

                foreach (MapHex adjacentHex in adjacentHexes)
                {
                    Edge edge = new Edge();
                    edge.Cost = 1;
                    edge.Length = Vector3.Distance(mapHex.transform.position,
                        adjacentHex.transform.position);
                    edge.ConnectedNode = adjacentHex.node;
                    edges.Add(edge);
                }
                mapHex.GetComponent<MapHex>().node.Connections = edges;
                nodes.Add(mapHex.GetComponent<MapHex>().node);
            }
        }

        manager.graphHexes.StartNode = manager.mouse.GetComponent<Mouse>().
            mouseHex.GetComponent<MapHex>().node;
        manager.graphHexes.EndNode = manager.mouse.GetComponent<Mouse>().
            tgtHex.GetComponent<MapHex>().node;
        manager.graphHexes.Nodes = nodes;
    }

    public List<Node> GetShortestPathAstar()
    { 
        shortestPath.Clear();
        foreach (var node in manager.graphHexes.Nodes)
        {
            node.StraightLineDistanceToEnd =
                node.StraightLineDistanceTo(manager.graphHexes.EndNode);
        }
        AstarSearch();
        shortestPath.Add(manager.graphHexes.EndNode);
        BuildShortestPath(shortestPath, manager.graphHexes.EndNode);
        shortestPath.Reverse();
        if (manager.graphHexes.StartNode.StraightLineDistanceToEnd > 5 &&
            shortestPath.Count() == 1)
        {
            shortestPath.Clear();
        }
        return shortestPath;
    }

    void BuildShortestPath(List<Node> list, Node node)
    { 
        if (node.NearestToStart == null ||
            node.NearestToStart == manager.graphHexes.StartNode)
            return;
        list.Add(node.NearestToStart);
        BuildShortestPath(list, node.NearestToStart);
    }

    void AstarSearch()
    {
        manager.graphHexes.StartNode.MinCostToStart = 0;
        var prioQueue = new List<Node>();
        prioQueue.Add(manager.graphHexes.StartNode);
        
        do
        {
            prioQueue = prioQueue.OrderBy(x =>
                x.MinCostToStart + x.StraightLineDistanceToEnd).ToList();
            var node = prioQueue.First();
            prioQueue.Remove(node);
            foreach (var cnn in node.Connections.OrderBy(x => x.Cost))
            {
                var childNode = cnn.ConnectedNode;
                if (childNode.Visited)
                    continue;
                if (childNode.MinCostToStart == null ||
                    node.MinCostToStart + cnn.Cost < childNode.MinCostToStart)
                {
                    childNode.MinCostToStart = node.MinCostToStart + cnn.Cost;
                    childNode.NearestToStart = node;
                    if (!prioQueue.Contains(childNode))
                        prioQueue.Add(childNode);
                }
            }
            node.Visited = true;
            if (node == manager.graphHexes.EndNode)
                return;
        } while (prioQueue.Any());
    }
    // ----------------------------------------------------------------------
    // END A* ALGORITHM CODE
}
