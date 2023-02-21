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

    public const float expandedColliderRadius = 8.175106f;
    //*************************************************************************

    /* PRIVATE VARS */
    //*************************************************************************
    private Manager manager;
    private int nodeVistis;
    private double ShortestPathLength;
    private double ShortestPathCost;
    public List<Node> shortestPath;
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
            //possibleMoves = manager.GetAdjacentHexes(transform.gameObject,
              //  1e-2f, expandedColliderRadius);

            //CheckLostCondition();

            if (!manager.userWin)
            {
                ComputeHexGraph();

                shortestPath = GetShortestPathAstar();

                /*transform.position = new Vector3(shortestPath[0].Point.X,
                    shortestPath[0].Point.Y, -1f);*/

                // DebugMoveToRandomHex();
                // Handle Mouse Win Condition ();
                manager.userTurn = true;
            }
        }
    }

    // Temp. Random Movement Until I make AI
    /*
    void DebugMoveToRandomHex()
    {
        chosenHex = Random.Range(0, possibleMoves.Count);
        transform.position = possibleMoves[chosenHex].transform.position +
            Vector3.back;
        possibleMoves.Clear();
    }
    */

    // Check list to see if it is empty - if so,
    // the mouse lost and disallow trying to move 
    /*
    void CheckLostCondition()
    {
        if (possibleMoves.Count == 0)
        {
            manager.userWin = true;
        }
    }
    */

    // Randomly choose target for mouse to go to
    void DebugEstablishTarget()
    {
        List<GameObject> edgeList = manager.mapHexes.FindAll(mapHex =>
            mapHex.GetComponent<MapHex>().isEdge == true);

        chosenHex = Random.Range(0, edgeList.Count);
        tgtHex = edgeList[chosenHex];
    }

    // Create the Map data structure used in the
    // shortest distance algorithm
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
                    edge.ConnectedNode = adjacentHex.node;
                    edges.Add(edge);
                }
                mapHex.GetComponent<MapHex>().node.Connections = edges;
                nodes.Add(mapHex.GetComponent<MapHex>().node);
            }
        }

        manager.graphHexes.StartNode = manager.mouse.GetComponent<Mouse>().
            mouseHex.GetComponent<MapHex>().node;
        Debug.Log(manager.graphHexes.StartNode.Connections.Count);
        manager.graphHexes.EndNode = manager.mouse.GetComponent<Mouse>().
            tgtHex.GetComponent<MapHex>().node;
        Debug.Log(manager.graphHexes.EndNode.Connections.Count);
        manager.graphHexes.Nodes = nodes;
    }

    public List<Node> GetShortestPathAstar()
    {
        foreach (var node in manager.graphHexes.Nodes)
        {
            node.StraightLineDistanceToEnd = node.StraightLineDistanceTo(manager.graphHexes.EndNode);
        }
        AstarSearch();
        var shortestPath = new List<Node>();
        shortestPath.Add(manager.graphHexes.EndNode);
        BuildShortestPath(shortestPath, manager.graphHexes.EndNode);
        shortestPath.Reverse();
        return shortestPath;
    }

    void BuildShortestPath(List<Node> list, Node node)
    {
        if (node.NearestToStart == null)
            return;
        list.Add(node.NearestToStart);
        ShortestPathLength += node.Connections.Single(x => x.ConnectedNode == node.NearestToStart).Length;
        ShortestPathCost += node.Connections.Single(x => x.ConnectedNode == node.NearestToStart).Cost;
        BuildShortestPath(list, node.NearestToStart);
    }

    void AstarSearch()
    {
        nodeVistis = 0;
        manager.graphHexes.StartNode.MinCostToStart = 0;
        var prioQueue = new List<Node>();
        prioQueue.Add(manager.graphHexes.StartNode);
        
        do
        {
            prioQueue = prioQueue.OrderBy(x => x.MinCostToStart + x.StraightLineDistanceToEnd).ToList();
            var node = prioQueue.First();
            prioQueue.Remove(node);
            nodeVistis++;
            Debug.Log(node.Connections.Count);
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
}
