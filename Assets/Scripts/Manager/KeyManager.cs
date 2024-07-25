using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeyManager : MonoBehaviour
{

    public static KeyManager instance;
    public event Action<Vector3> move;
    public event Action restart;
    public event Action<int> turnView;
    private Vector3[] moveDirections = { new Vector3(0, 0, -1), new Vector3(1, 0, 0), new Vector3(0, 0, 1), new Vector3(-1, 0, 0) };
    private int moveDirIndex = 0;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }
    private void Update()
    {
        keyControl();
    }
    public void keyControl()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                move?.Invoke(moveDirections[moveDirIndex]);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                move?.Invoke(moveDirections[(moveDirIndex + 1) % 4]);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                move?.Invoke(moveDirections[(moveDirIndex + 2) % 4]);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                move?.Invoke(moveDirections[(moveDirIndex + 3) % 4]);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                restart?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                moveDirIndex = (moveDirIndex + 3) % 4;
                turnView?.Invoke(-1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                moveDirIndex = (moveDirIndex + 1) % 4;
                turnView?.Invoke(1);
            }
        }
    }
}
