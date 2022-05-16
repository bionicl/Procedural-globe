using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public float speed = 6;
    public float mouseSensitivity = 2f;
    public float boostSpeed;

    private void Update()
    {
        Move();
        Rotate();
    }

    void Move()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = boostSpeed;
        }

        Vector3 move = transform.forward * verticalMove + transform.right * horizontalMove;
        characterController.Move(currentSpeed * Time.deltaTime * move);
    }

    public void Rotate()
    {
        float horizontalRotation = Input.GetAxis("Mouse X");
        float verticalRotation = Input.GetAxis("Mouse Y");
        //verticalRotation = Mathf.Clamp(verticalRotation, downLimit, upLimit);
        transform.Rotate(0, horizontalRotation * mouseSensitivity, 0);
    }
}
