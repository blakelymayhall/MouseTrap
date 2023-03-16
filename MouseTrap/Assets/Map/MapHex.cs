using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHex : MonoBehaviour
{
    /* PUBLIC VARS */
    //*************************************************************************
    public bool isClicked    = false;
    public bool isMouseOn    = false;
    public bool inaccessible = false;
    public bool isEdge       = false;
    public Node node         = new Node();  // used in A* shortest path algorithm

    public const float nominalColliderRadius = 2.340276f;
    public const float expandedColliderRadius = 6f;
    //*************************************************************************

    /* PRIVATE VARS */
    //*************************************************************************
    private Manager manager;
    //*************************************************************************

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponentInParent<Manager>();

        Point point = new Point();
        point.X = transform.position.x;
        point.Y = transform.position.y;
        node.Id = Guid.NewGuid();
        node.Point = point;
    }

    // Update is called once per frame
    void Update()
    {
        if (!manager.userWin && !manager.mouseWin)
        {
            CheckMouseOn();
            CheckForUserClick();
        }
    }

    // Checks if mouse is on the Hex 
    void CheckMouseOn()
    {
        if (GameObject.Find("Mouse").GetComponent<CircleCollider2D>().
            OverlapPoint(transform.position))
        {
            isMouseOn = true;
            manager.mouse.GetComponent<Mouse>().mouseHex =
                transform.gameObject;
        }
        else
        {
            isMouseOn = false;
        }
    }

    // Checks if user clicked
    void CheckForUserClick()
    {
        // If a click was made, check if we clicked a hex, if so, turn it black
        // and flip isClicked property
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit_detected = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                Vector2.zero);

            if (hit_detected && !isMouseOn && !isClicked)
            {
                if (hit_detected.collider.gameObject == transform.gameObject)
                {
                    isClicked = true;
                    manager.userTurn = false;
                    Manager.numClicks++;
                    GetComponent<SpriteRenderer>().color = Color.black;
                }
                
            }
        }
    }
}
