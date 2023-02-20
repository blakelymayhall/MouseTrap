using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGame : MonoBehaviour
{
    /* PUBLIC VARS */
    //*************************************************************************
    public GameObject mapHex_Prefab;
    public GameObject mouse_Prefab;

    public int Radius = 5;
    //*************************************************************************

    /* PRIVATE VARS */
    //*************************************************************************
    private Manager manager;
    private const float sq3 = 1.7320508075688772935274463415059F;
    //*************************************************************************

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<Manager>();
        GenerateMap();
        SpawnMouse();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Generate the Hexes that make the grid 
    void GenerateMap()
    {
        // Make first hex @ origin
        // Vector should be z = 0 so it shows below mouse
        Vector3 spawnPosition = new Vector3(0f, 0f, 0f);
        GameObject originHex = Instantiate(mapHex_Prefab, spawnPosition,
            Quaternion.identity, GetComponent<Transform>());
        originHex.name = "Hex";

        /*
        Code stolen from:
        https://www.codeproject.com/
        Articles/1249665/Generation-of-a-hexagonal-tessellation
        */

        //Spawn scheme: nDR, nDX, nDL, nUL, nUX, End??, UX, nUR
        Vector3[] mv = {
            new Vector3(1.5f,-sq3*0.5f, 0),       //DR
            new Vector3(0,-sq3, 0),               //DX
            new Vector3(-1.5f,-sq3*0.5f, 0),      //DL
            new Vector3(-1.5f,sq3*0.5f, 0),       //UL
            new Vector3(0,sq3, 0),                //UX
            new Vector3(1.5f,sq3*0.5f, 0)         //UR
        };

        // 2.8f found experimentally
        // would need to change when sprite changes
        int lmv = mv.Length;
        float HexSide = mapHex_Prefab.transform.localScale.x * 2.8f;

        // Make counter and calc. when on final radius to apply .isEdge param
        // in MapHex
        int counter = 1;
        int edgeBegins = 1;
        for (int ii = 1; ii < Radius; ii++)
        {
            edgeBegins += 6 * ii;
        }

        // Exec algorithm 
        Vector3 currentPoint = new Vector3(0f, 0f, 0f);
        for (int mult = 0; mult <= Radius; mult++)
        {
            for (int j = 0; j < lmv; j++)
            {
                for (int i = 0; i < mult; i++)
                {
                    currentPoint += (mv[j] * HexSide);
                    GameObject h = Instantiate(mapHex_Prefab, currentPoint,
                        mapHex_Prefab.transform.rotation, transform);
                    h.name = "Hex";
                    manager.mapHexes.Add(h);
                    counter++;
                    if (counter >= edgeBegins)
                    {
                        h.GetComponent<MapHex>().isEdge = true;
                    }
                }
                if (j == 4)
                {
                    if (mult == Radius)
                        break;      //Finished
                    currentPoint += (mv[j] * HexSide);
                    GameObject h = Instantiate(mapHex_Prefab, currentPoint,
                        mapHex_Prefab.transform.rotation, transform);
                    h.name = "Hex";
                    manager.mapHexes.Add(h);
                    counter++;
                    if (counter >= edgeBegins)
                    {
                        h.GetComponent<MapHex>().isEdge = true;
                    }
                }
            }
        }
    }

    // Create the Map data structure used in the
    // shortest distance algorithm

    // This should go in manager when you are done maybe?
    // def. not in load game since that implies one and done 
    void ComputeHexGraph()
    {


        List<Node> nodes = new List<Node>();
        foreach (GameObject mapHex in manager.mapHexes)
        {
            Node node = new Node();

            Point point = new Point();
            point.X = mapHex.transform.position.x;
            point.Y = mapHex.transform.position.y;

            node.Id = Guid.NewGuid();
            node.Point = point;
        }
            /*
            // Each node has at most 6 edges
            List<Edge> edges = new List<Edge>();
            List<MapHex> adjacentHexes = manager.GetAdjacentHexes(mapHex,
                MapHex.nominalColliderRadius,
                MapHex.expandedColliderRadius);
            foreach(MapHex adjacentHex in adjacentHexes)
            {
                Edge edge = new Edge();
                edge.ConnectedNode = 
            }

            Node node = new Node();

            Point point = new Point();
            point.X = mapHex.transform.position.x;
            point.Y = mapHex.transform.position.y;

            node.Id = Guid.NewGuid();
            node.Point = point;
            //node.Connections =
            
        }


            */

        //manager.graphHexes.StartNode =
        //manager.graphHexes.EndNode =
        //manager.graphHexes.Nodes = 
    }

    // Generate the mouse
    void SpawnMouse()
    {
        // Make first hex @ origin
        // Vector should be z = -1 so it shows on top of the hexes
        Vector3 spawnPosition = new Vector3(0f, 0f, -1f);
        GameObject mouse = Instantiate(mouse_Prefab, spawnPosition,
            Quaternion.identity, GetComponent<Transform>());
        mouse.name = "Mouse";
        manager.mouse = mouse;
    }
}
