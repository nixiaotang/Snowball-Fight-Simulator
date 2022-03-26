using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMultiplayer : MonoBehaviour
{

    //[SerializeField] private GameObject player;

    private Quaternion cameraRot;
    private Vector3 globalPos;
    private Vector3 startPlayerPos;

    [SerializeField] private Transform player;

    private void Start()
    {
        cameraRot = transform.rotation;
        startPlayerPos = player.position;
        globalPos = new Vector3(20f, 5.77f, 3f);

    }

    void Update()
    {

        transform.rotation = cameraRot;
        transform.position = globalPos + (player.position - startPlayerPos);

    }
}
