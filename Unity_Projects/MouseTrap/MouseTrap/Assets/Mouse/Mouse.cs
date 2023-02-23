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
    private double shortestPathLength;
    private double shortestPathCost;
    //*************************************************************************


    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponentInParent<Manager>();
        DebugEstablishTarget();
    }

    // Update is called once per frame
    void Update()
    {
        // Disallow mouse movement unless its the mouse's turn
        if (!manager.userTurn)
        {
            //CheckLostCondition();
            if (!manager.userWin)
            {
                DebugEstablishTarget();
                Debug.Log("---");
                Debug.Log("TGT");
                Debug.Log(tgtHex.transform.position.x);
                Debug.Log(tgtHex.transform.position.y);
                Debug.Log("--TGT");

                shortestPath.Clear();
                shortestPathCost = 0;
                shortestPathLength = 0;
                foreach (GameObject mapHex in manager.mapHexes)
                {
                    mapHex.GetComponent<MapHex>().node.MinCostToStart = null;
                    mapHex.GetComponent<MapHex>().node.Visited = false;
                    mapHex.GetComponent<MapHex>().node.StraightLineDistanceToEnd = 0;
                    mapHex.GetComponent<MapHex>().node.NearestToStart = null;
                }

                ComputeHexGraph();

                shortestPath = GetShortestPathAstar();
                //BlunderLogic();
                foreach(Node tmp in shortestPath)
                {
                    Debug.Log("shtp");
                    Debug.Log(tmp.Point.X);
                    Debug.Log(tmp.Point.Y);
                    Debug.Log("--shtp");
                }

                transform.position = new Vector3(shortestPath[0].Point.X,
                    shortestPath[0].Point.Y, transform.position.z);

                //CheckWinCondition();
                manager.userTurn = true;
            }
        }
    }

    // Check list to see if it is shortest path is one - if so,
    // the mouse won and disallow trying to move 
    void CheckWinCondition()
    {
        if (shortestPath.Count == 1)
        {
            // do stuff
        }
    }

    // Randomly choose target for mouse to go to
    void DebugEstablishTarget()
    {
        /* SHOULD FIND BETTER WAY TO DO THIS*/
        List<GameObject> edgeList = manager.mapHexes.FindAll(mapHex =>
            mapHex.GetComponent<MapHex>().isEdge == true &&
            !mapHex.GetComponent<MapHex>().isClicked);

        chosenHex = Random.Range(0, edgeList.Count);
        tgtHex = edgeList[chosenHex];
    }

    // Randomly fuck up shortest path
    void BlunderLogic()
    {
        if(Random.Range(0, 10) > 5)
        {
            Debug.Log("BLUNDER");
            List<MapHex> adjHex =
                manager.GetAdjacentHexes(manager.mouse.GetComponent<Mouse>().
                mouseHex, MapHex.nominalColliderRadius,
                MapHex.expandedColliderRadius);
            shortestPath[0] = adjHex[Random.Range(0, adjHex.Count)].node;
        }
    }

    // BEGIN A* ALGORITHM
    // ----------------------------------------------------------------------
    // Create the Map data structure used in the A* Algorithm
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
            node.StraightLineDistanceToEnd = node.StraightLineDistanceTo(manager.graphHexes.EndNode);
        }
        AstarSearch();
        shortestPath.Add(manager.graphHexes.EndNode);
        BuildShortestPath(shortestPath, manager.graphHexes.EndNode);
        shortestPath.Reverse();
        return shortestPath;
    }

    void BuildShortestPath(List<Node> list, Node node)
    {
        Debug.Log("bsp:");
        Debug.Log(node.Point.X);
        Debug.Log(node.Point.Y);
        Debug.Log("--bsp:");

        if (node.NearestToStart == null)
            return;
        var tmp = node.Connections.
            FirstOrDefault(x => x.ConnectedNode == node.NearestToStart);
        if (tmp == null)
            return;
        shortestPathLength += tmp.Length;
        var tmp2 = node.Connections.
            FirstOrDefault(x => x.ConnectedNode == node.NearestToStart);
        if (tmp == null)
            return;
        list.Add(node.NearestToStart);
        shortestPathCost += tmp.Cost;
        BuildShortestPath(list, node.NearestToStart);
    }

    void AstarSearch()
    {
        manager.graphHexes.StartNode.MinCostToStart = 0;
        var prioQueue = new List<Node>();
        prioQueue.Add(manager.graphHexes.StartNode);
        
        do
        {
            prioQueue = prioQueue.OrderBy(x => x.MinCostToStart + x.StraightLineDistanceToEnd).ToList();
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
