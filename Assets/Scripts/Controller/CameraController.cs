using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3[] viewDirections = { new Vector3(-1, -1, 1), new Vector3(1, -1, 7), new Vector3(1, 1, 5), new Vector3(-1, 1, 3) };
    private int viewDirIndex = 0;
    private void Awake()
    {
        cameraTransform = GetComponent<Transform>();
        Vector3 playerPosition = PlayerController.instance.playerTransform.position;
        cameraTransform.position = new Vector3(playerPosition.x - 3, playerPosition.y + 1.25f, playerPosition.z - 3);
    }
    private void Start()
    {
        KeyManager.instance.turnView += turnView;
    }
    void Update()
    {
        Vector3 playerPosition = PlayerController.instance.playerTransform.position;
        cameraTransform.position = new Vector3(playerPosition.x + 3 * viewDirections[viewDirIndex].x, playerPosition.y + 1.25f, playerPosition.z + 3 * viewDirections[viewDirIndex].y);
    }
    void turnView(int turnDirection)
    {
        viewDirIndex = (viewDirIndex + turnDirection + 4) % 4;
        cameraTransform.rotation = Quaternion.Euler(20, 45 * viewDirections[viewDirIndex].z, 0);
    }
}
