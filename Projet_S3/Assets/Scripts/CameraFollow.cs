using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // The character to follow
    [SerializeField] private Vector3 offset = new Vector3(0.7f, 1.5f, -1.4f); // Offset position of the camera
    [SerializeField] private float positionSmoothSpeed = 0.125f; // Smoothing speed for camera movement
    [SerializeField] private float rotationSmoothSpeed = 5f; // Smoothing speed for camera rotation

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate the desired position based on the target's rotation and offset
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;

        // Calculate the desired rotation to align with the target's forward direction
        Quaternion desiredRotation = Quaternion.LookRotation(target.forward, Vector3.up);

        // Smoothly interpolate between the current rotation and the desired rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothSpeed);
    }
}
