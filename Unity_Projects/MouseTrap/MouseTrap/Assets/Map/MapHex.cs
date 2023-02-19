using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHex : MonoBehaviour
{
    /* PUBLIC VARS */
    //*************************************************************************
    public bool isClicked = false;
    public bool isMouseOn = false;          
    public bool isEdge    = false;                 
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
        CheckMouseOn();
        CheckMouseClicked();
    }

    void CheckMouseOn()
    {
        isMouseOn = false;
        if (GameObject.Find("Mouse").GetComponent<CircleCollider2D>().
            OverlapPoint(transform.position))
        {
            isMouseOn = true;
        }
    }

    void CheckMouseClicked()
    {
        // If a click was made, check if we clicked a hex
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
                    GetComponent<SpriteRenderer>().color = Color.black;
                }
                
            }
        }
    }
}
