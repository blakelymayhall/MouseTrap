using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    /* PUBLIC VARS */
    //*************************************************************************
    public List<MapHex> possibleMoves = new List<MapHex>();
    public int chosenHex;

    public const float expandedColliderRadius = 8.175106f;
    //*************************************************************************

    /* PRIVATE VARS */
    //*************************************************************************
    private Manager manager;
    //*************************************************************************


    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponentInParent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Disallow mouse movement unless its the mouse's turn
        if (!manager.userTurn)
        {
            possibleMoves = manager.GetAdjacentHexes(transform.gameObject,
                1e-2f, expandedColliderRadius);
            CheckLostCondition();

            if (!manager.userWin)
            {
                DebugMoveToRandomHex();
                //Handle Mouse Win Condition ();
                manager.userTurn = true;
            }
        }
    }

    // Temp. Random Movement Until I make AI
    void DebugMoveToRandomHex()
    {
        chosenHex = Random.Range(0, possibleMoves.Count);
        transform.position = possibleMoves[chosenHex].transform.position +
            Vector3.back;
        possibleMoves.Clear();
    }

    // Check list to see if it is empty - if so,
    // the mouse lost and disallow trying to move 
    void CheckLostCondition()
    {
        if (possibleMoves.Count == 0)
        {
            manager.userWin = true;
        }
    }


}
