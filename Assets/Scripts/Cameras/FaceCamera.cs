using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // This script rotates some piece of UI towards the camera

    private Transform mainCameraTransform;
    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    // This code makes some piece of UI face the camera at all times.
    private void LateUpdate()
    {
        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
            mainCameraTransform.rotation * Vector3.up);
    }
}
