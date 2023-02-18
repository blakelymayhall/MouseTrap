using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    /* PUBLIC VARS */
    //*************************************************************************
    public List<MapHex> possibleMoves = new List<MapHex>();
    public int chosenHex;
    //*************************************************************************

    /* PRIVATE VARS */
    //*************************************************************************
    private Manager manager;
    private float mouseColliderRadius = 8.175106f;
    //*************************************************************************


    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponentInParent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        possibleMoves.Clear();
        if (!manager.userTurn)
        { 
            GetComponent<CircleCollider2D>().radius = mouseColliderRadius;
            GetPossibleMoves();
            CheckLostCondition();
            if (!manager.userWin)
            {
                DebugMoveToRandomHex();
                GetComponent<CircleCollider2D>().radius = 1e-2f;

                //Handle Mouse Win Condition 
                manager.userTurn = true;
            }
        }
    }

    // Finds all adjacent hexes
    void GetPossibleMoves()
    {
        List<Collider2D> results = new List<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        GetComponent<CircleCollider2D>().
            OverlapCollider(contactFilter.NoFilter(), results);

        foreach(Collider2D result in results)
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
                if (!mapHex.isClicked)
                {
                    possibleMoves.Add(mapHex);
                }
            }
        }
    }

    // Temp. Random Movement Until I make AI
    // force move to winning hex 
    void DebugMoveToRandomHex()
    {
        chosenHex = Random.Range(0, possibleMoves.Count);
        transform.position = possibleMoves[chosenHex].transform.position;
    }

    // Check list to see if it is empty - if so,
    // the mouse lost
    void CheckLostCondition()
    {
        if (possibleMoves.Count == 0)
        {
            manager.userWin = true;
        }
    }

}
