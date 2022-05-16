using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public float speed = 6;
    public float mouseSensitivity = 2f;
    public float upLimit = -50;
    public float downLimit = 50;

    private void Update()
    {
        Move();
        Rotate();
    }

    void Move()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * verticalMove + transform.right * horizontalMove;
        characterController.Move(speed * Time.deltaTime * move);
    }

    public void Rotate()
    {
        float horizontalRotation = Input.GetAxis("Mouse X");
        float verticalRotation = Input.GetAxis("Mouse Y");
        //verticalRotation = Mathf.Clamp(verticalRotation, downLimit, upLimit);
        transform.Rotate(0, horizontalRotation * mouseSensitivity, 0);
    }
}
