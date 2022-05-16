using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public float speed = 6;
    public float mouseSensitivity = 2f;
    public float flyingSpeed = 40;
    public float boostSpeed;

    private void Update()
    {
        Move();
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            Rotate();
    }

    void Move()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");
        float yAxisMove = 0;
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            yAxisMove = -1;
        } else if (Input.GetKey(KeyCode.Space))
        {
            yAxisMove = 1;
        }
        //if (Input.GetKey(KeyCode.LeftShift))
        //{
        //    currentSpeed = boostSpeed;
        //}

        float zPos = transform.position.y;
        Vector3 move = transform.forward * verticalMove + transform.right * horizontalMove;
        characterController.Move(currentSpeed * Time.deltaTime * move);
        if (yAxisMove > 0.01 || yAxisMove < -0.01)
        {
            zPos += Time.deltaTime * yAxisMove * flyingSpeed;
        } else
        {
            zPos = transform.position.y;
        }
        transform.position = new Vector3(transform.position.x, zPos, transform.position.z);
    }

    public void Rotate()
    {
        float horizontalRotation = Input.GetAxis("Mouse X");
        float verticalRotation = Input.GetAxis("Mouse Y");
        //verticalRotation = Mathf.Clamp(verticalRotation, downLimit, upLimit);
        transform.Rotate(-verticalRotation * mouseSensitivity, horizontalRotation * mouseSensitivity, 0);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }
}
