using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController controller;
    public GameObject cameraChild;

    public float ms;
    public float xSens;
    public float ySens;
    public float jumpHeight;
    public float fallSpeed;
    public float sprintSpeed;
    public bool enableGrav;

    private Vector3 velocity; 
    private float g = -9.82f;

    private void Start() {
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update() {
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.green);
        Debug.DrawLine(transform.position, transform.position + transform.right, Color.blue);

        handleMouse();
        handleMovement();
    }

    private void handleMouse() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        if (Cursor.lockState == CursorLockMode.None) { return; }

        float mouseX = Input.GetAxis("Mouse X");
        if (mouseX != 0) {
            transform.Rotate( 0, mouseX * Time.deltaTime * xSens, 0 );
        }

        float mouseY = Input.GetAxis("Mouse Y");
        if (mouseY != 0) { 
            float ang = mouseY * Time.deltaTime * ySens;
            cameraChild.transform.RotateAround(cameraChild.transform.position, -transform.right, ang);
        }
    }

    private void handleMovement() {
        var groundedPlayer = controller.isGrounded;
        if (groundedPlayer && velocity.y < 0) {
            velocity.y = 0f;
        }

        int forwardDir = Convert.ToInt32(Input.GetKey(KeyCode.W)) * 2 - 1;
        forwardDir -= Convert.ToInt32(Input.GetKey(KeyCode.S)) * 2 - 1;

        int sideDir = Convert.ToInt32(Input.GetKey(KeyCode.D)) * 2 - 1;
        sideDir -= Convert.ToInt32(Input.GetKey(KeyCode.A)) * 2 - 1;

        Vector3 mv = (transform.forward * forwardDir) + (transform.right * sideDir);

        if (mv != Vector3.zero) {
            float sprintMod = (Input.GetKey(KeyCode.LeftShift) == true) ? sprintSpeed : 1;
            controller.Move(mv * Time.deltaTime * ms * sprintMod);
        }

        if (Input.GetKey(KeyCode.Space) && groundedPlayer) {
            velocity.y += Mathf.Sqrt(jumpHeight * -fallSpeed * g);
        }

        if (enableGrav) { velocity.y += g * Time.deltaTime; }
        if (velocity != Vector3.zero) {
            controller.Move(velocity * Time.deltaTime);
        }
    }
}