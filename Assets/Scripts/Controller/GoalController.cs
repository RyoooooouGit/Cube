using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoalController : MonoBehaviour
{
    private Transform goalTransform;
    public Transform destination;
    private void Awake()
    {
        goalTransform = GetComponent<Transform>();
    }
    void Update()
    {
        Transform playerTransform = PlayerController.instance.playerTransform;
        if (!PlayerController.instance.ifTeleporting && goalTransform.position.Equals(playerTransform.position) && goalTransform.rotation.eulerAngles.Equals(playerTransform.rotation.eulerAngles))
        {
            StartCoroutine(PlayerController.instance.winIEnumerator(destination));
        }
    }

}
