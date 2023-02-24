using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    /* PUBLIC VARS */
    //*************************************************************************
    //public List<MapHex> possibleMoves = new List<MapHex>();
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
    //*************************************************************************


    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponentInParent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Disallow mouse movement unless its the mouse's turn and the
        if (!manager.userTurn)
        {
            CheckWinLoss();
            if (!manager.userWin)
            {
                EstablishTarget();
                ResetAstarAlgorithm();
                ComputeHexGraph();
                MoveMouse();
            }

            manager.userTurn = true;
        }
    }

    // Randomly choose target for mouse to go to on first pass, then
    // if target gets clicked choose nearest adjacent targets 
    void EstablishTarget()
    {
        if (potentialTargetList.Count() == 0)
        {
            potentialTargetList = manager.mapHexes.FindAll(mapHex =>
                mapHex.GetComponent<MapHex>().isEdge == true &&
                !mapHex.GetComponent<MapHex>().isClicked);
            chosenHex = Random.Range(0, potentialTargetList.Count);
            tgtHex = potentialTargetList[chosenHex];
        }
        else
        {
            while (tgtHex.GetComponent<MapHex>().isClicked)
            {
                List<MapHex> adjHex = manager.GetAdjacentHexes(tgtHex,
                    MapHex.nominalColliderRadius,
                    MapHex.expandedColliderRadius, true);
                var newTgtHex = adjHex.FirstOrDefault(hex =>
                    hex.isEdge == true && !hex.isClicked);
                if (newTgtHex != null)
                {
                    tgtHex = newTgtHex.gameObject;
                }
                else
                {
                    tgtHex = adjHex.FirstOrDefault(hex =>
                        hex.isEdge == true && hex.isClicked).gameObject;
                }
            }
        }
        tgtHex.GetComponent<SpriteRenderer>().color = Color.green;
    }

    // Randomly fuck up shortest path by choosing an adjacent hex at random
    void BlunderLogic()
    {
        if (Random.Range(0, 101) > manager.mouseBlunderPercentage)
        {
            List<MapHex> adjHex =
                manager.GetAdjacentHexes(manager.mouse.GetComponent<Mouse>().
                mouseHex, MapHex.nominalColliderRadius,
                MapHex.expandedColliderRadius);
            shortestPath[0] = adjHex[Random.Range(0, adjHex.Count)].node;
        }
    }

    // Move mouse in direction of shortest path. If shortest path undetermined
    // i.e. target is unreachable, then randomize movement and retarget on next
    // turn
    void MoveMouse()
    {
        shortestPath = GetShortestPathAstar();
        if (shortestPath.Count() > 1)
        {
            BlunderLogic();
        }
        else
        {
            potentialTargetList.Clear(); // Force retargeting
            List<MapHex> adjHex =
                manager.GetAdjacentHexes(manager.mouse.GetComponent<Mouse>().
                mouseHex, MapHex.nominalColliderRadius,
                MapHex.expandedColliderRadius);
            shortestPath.Add(adjHex[Random.Range(0, adjHex.Count)].node);
        }
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
    }

    // Randomly select a target in the event the 

    /* BEGIN A* ALGORITHM

    ResetAstarAlgorithm loops through the MapHexes saved in the manager
    object and resets their internal parameters

    ComputeHexGraph establishes the connected nodes given that the map
    changes everytime the user clicks a hex

    GetShortestPathAstar calls AstarSeach and BuildShortestPath to actually
    generate the shortest path to the target 

    */
    // ----------------------------------------------------------------------
    void ResetAstarAlgorithm()
    {
        shortestPath.Clear();
        foreach (GameObject mapHex in manager.mapHexes)
        {
            mapHex.GetComponent<MapHex>().node.MinCostToStart = null;
            mapHex.GetComponent<MapHex>().node.Visited = false;
            mapHex.GetComponent<MapHex>().node.StraightLineDistanceToEnd = 0;
            mapHex.GetComponent<MapHex>().node.NearestToStart = null;
            mapHex.GetComponent<MapHex>().node.Connections.Clear();
        }
    }

    void ComputeHexGraph()
    {
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
