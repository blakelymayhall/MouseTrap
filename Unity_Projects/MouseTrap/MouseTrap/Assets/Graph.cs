using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Entire Map Object Containing Start Position, Connected Nodes,
// and End Position
public class Graph
{
    public List<Node> Nodes { get; set; } = new List<Node>();

    public Node StartNode { get; set; }

    public Node EndNode { get; set; }

    public List<Node> ShortestPath { get; set; } = new List<Node>();
}

// Node's Within the Map
public class Node
{
    public Guid Id { get; set; }
    public Point Point { get; set; }
    public List<Edge> Connections { get; set; } = new List<Edge>();

    public double? MinCostToStart { get; set; }
    public Node NearestToStart { get; set; }
    public bool Visited { get; set; }
    public double StraightLineDistanceToEnd { get; set; }

    public double StraightLineDistanceTo(Node end)
    {
        return Math.Sqrt(Math.Pow(Point.X - end.Point.X, 2) + Math.Pow(Point.Y - end.Point.Y, 2));
    }
}

// Edges Connecting the Nodes
public class Edge
{
    public double Length { get; set; }
    public double Cost { get; set; }
    public Node ConnectedNode { get; set; }
}

// X,Y points for Nodes
public class Point
{
    public float X { get; set; }
    public float Y { get; set; }
}
