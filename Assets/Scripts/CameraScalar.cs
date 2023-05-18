﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScalar : MonoBehaviour
{

    private Board board;
    public float camerOffset;
    public float aspectRatio = 0.625f;
    public float padding = 2;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.width-1, board.height-1);

        }
    }

    void RepositionCamera(float x, float y)
    {//we want camera to be directly in the middle and halfway up to where the candy are
        Vector3 tempPosition = new Vector3(x/2, y/2, camerOffset); // creates temporary postion 
       // enacting the tempposistion
        transform.position = tempPosition;
        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + padding;
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
