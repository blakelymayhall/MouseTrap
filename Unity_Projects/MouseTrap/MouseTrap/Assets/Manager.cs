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

    public int mouseBlunderPercentage = 20;
    public bool userTurn;
    public bool mouseWin;
    public bool userWin;
    //*************************************************************************

    /* PRIVATE VARS */
    //*************************************************************************

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
}
