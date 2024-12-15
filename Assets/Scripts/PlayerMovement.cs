using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Movement force of player in newtons per second")]
    [Range(0,1000)]
    private float movementSpeed;

    [SerializeField]
    [Tooltip("Rotation speed of player in newtons per second")]
    [Range(0, 100)]
    private float rotationSpeed;

    private Rigidbody rb;
    
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float space = movementSpeed * Time.deltaTime;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical);
        
        rb.AddRelativeForce(direction.normalized * space);
        
        float angle = rotationSpeed * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X");
        
        rb.AddRelativeTorque(0, mouseX * angle, 0);
    }
}
