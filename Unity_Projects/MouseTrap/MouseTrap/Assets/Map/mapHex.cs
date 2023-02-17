using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapHex : MonoBehaviour
{
    /* PUBLIC VARS */
    //*************************************************************************
    public bool isClicked = false;
    public bool isMouseOn;          // Set by generation?
    public bool isEdge;             // May not need? Set by generation?
    //*************************************************************************

    /* PRIVATE VARS */
    //*************************************************************************

    //*************************************************************************

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckMouseClicked();
    }

    void CheckMouseClicked()
    {
        // If a click was made, check if we clicked a hex
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit_detected = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                Vector2.zero);

            if (hit_detected)
            {
                if (hit_detected.collider.gameObject == transform.gameObject)
                {
                    isClicked = true;
                    GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
        }
    }
}
