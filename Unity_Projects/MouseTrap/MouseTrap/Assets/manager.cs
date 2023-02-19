using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    /* PUBLIC VARS */
    //*************************************************************************
    public GameObject mapHex_Prefab;
    public GameObject mouse_Prefab;

    public bool userTurn;
    public bool mouseWin;
    public bool userWin;

    public int Radius = 5;
    //*************************************************************************

    /* PRIVATE VARS */
    //*************************************************************************
    private const float sq3 = 1.7320508075688772935274463415059F;
    //*************************************************************************

    // Start is called before the first frame update
    void Start()
    {
        // Can this definition be within a map.c script inside the map folder? 
        GenerateMap();

        // Spawn mouse -- can this definition be from mouse.c?
        SpawnMouse();

        // User's turn first
        userTurn = true;
        mouseWin = false;
        userWin  = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnMouse()
    {
        // Make first hex @ origin
        Vector3 spawnPosition = new Vector3(0f, 0f, -1f);
        GameObject mouse = Instantiate(mouse_Prefab, spawnPosition,
            Quaternion.identity, GetComponent<Transform>());
        mouse.name = "Mouse";
    }

    void GenerateMap()
    {
        // Make first hex @ origin
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
        // or maybe even screen?;
        int lmv = mv.Length;
        float HexSide = mapHex_Prefab.transform.localScale.x*2.8f; 
                                                                   
                                                                   
        // Make counter and calc. when on final radius to apply .isEdge param
        // in MapHex
        int counter = 1;
        int edgeBegins = 1;
        for(int ii = 1; ii < Radius; ii++)
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
                    counter++;
                    if (counter >= edgeBegins)
                    {
                        h.GetComponent<MapHex>().isEdge = true;
                    }
                }
            }
        }
    }
}
