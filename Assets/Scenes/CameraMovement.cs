using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float rotationSpeed = 100.0f;

    private float horizontalRotation = 0.0f;

    void Update()
    {
        // Move the camera based on WASD keys
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime);

        // Rotate the camera based on mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        horizontalRotation += mouseX * rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, horizontalRotation, 0);
    }
}
