using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    /* PUBLIC VARS */
    //*************************************************************************
    [System.NonSerialized]
    public GameObject mouse;

    [System.NonSerialized]
    public List<GameObject> mapHexes = new List<GameObject>();

    [System.NonSerialized]
    public Graph graphHexes = new Graph();

    [System.NonSerialized]
    public int mouseBlunderPercentage = 20;

    [System.NonSerialized]
    public int numAlreadyClicked = 20;

    public GameObject wl_menu_prefab;
    public GameObject wl_menu;
    public bool userTurn;
    public bool mouseWin;
    public bool userWin;
    //*************************************************************************

    /* PRIVATE VARS */
    //*************************************************************************
    private bool wl_menu_loaded = false;
    //*************************************************************************

    // Start is called before the first frame update
    void Start()
    {
        // User's turn first
        userTurn = true;
        mouseWin = false;
        userWin  = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (userWin || mouseWin && !wl_menu_loaded)
        {
            // Make a picture that is empty around the border to use as canvas
            // background


            //LoadWL_Menu();
            //wl_menu_loaded = true;
        }
    }

    public List<MapHex> GetAdjacentHexes(GameObject originObject,
        float nominalColliderRadius,
        float expandedColliderRadius, bool allowClicked = false)
    {
        List<MapHex> adjacentHexes = new List<MapHex>();

        // Grow circle collider so it can find adjacent hexes
        originObject.GetComponent<CircleCollider2D>().radius =
            expandedColliderRadius;

        // Get list of overlapping colliders
        List<Collider2D> results = new List<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        originObject.GetComponent<CircleCollider2D>().
            OverlapCollider(contactFilter.NoFilter(), results);

        foreach (Collider2D result in results)
        {
            // Check if the circle collider is overlapping a collider
            // belonging to a hex

            // then check to make sure it is not the hex the mouse is currently
            // on

            // then check to make sure the hex isn't clicked

            bool isHex = result.transform.name == "Hex";
            bool isAdjacent = (result.transform.position - transform.position)
                .magnitude > 1e-2f;

            if (isHex && isAdjacent)
            {
                MapHex mapHex = result.GetComponentInParent<MapHex>();
                if (allowClicked)
                {
                    adjacentHexes.Add(mapHex);
                }
                else if(!mapHex.isClicked)
                {
                    adjacentHexes.Add(mapHex);
                }
            }
        }

        // Shrink circle collider so mouse cannot strike it instead of
        // a hex
        originObject.GetComponent<CircleCollider2D>().radius =
            nominalColliderRadius;


        return adjacentHexes;
    }

    void LoadWL_Menu()
    {
        wl_menu = Instantiate(wl_menu_prefab, -3*Vector3.forward,
                                 Quaternion.identity, GetComponent<Transform>());
        wl_menu.name = "wl_menu";
        wl_menu.GetComponent<Canvas>().worldCamera = Camera.main;
        wl_menu.GetComponent<Canvas>().planeDistance = 1;
        //wl_menu.GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 0f);
        //wl_menu.GetComponent<RectTransform>().sizeDelta = new Vector2(0.75f, 0.75f);
    }
}
