using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private float maxVelocity = 10f;
    [SerializeField]
    [Tooltip("Movement force of player in newtons per second")]
    [Range(0,1000)]
    private float movementSpeed;

    [SerializeField]
    [Tooltip("Rotation speed of player in newtons per second")]
    [Range(0, 100)]
    private float rotationSpeed;

    private Rigidbody _rb;
    private Animator _animator;
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        float space = movementSpeed * Time.deltaTime;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical);
        
        _rb.AddRelativeForce(direction.normalized * space);
        
        float angle = rotationSpeed * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X");
        
        _rb.AddRelativeTorque(0, mouseX * angle, 0);
        

        _animator.SetFloat("MoveX", horizontal);
        _animator.SetFloat("MoveY", vertical);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            float normalizedVelocity = Mathf.Clamp01(_rb.velocity.magnitude / maxVelocity);
            _animator.SetFloat("Velocity", normalizedVelocity);
        }
        else
        {
            if (Mathf.Abs(horizontal) < 0.01f && Mathf.Abs(vertical) < 0.01f)
            {
                _animator.SetFloat("Velocity",0);
            }
            else
            {
                _animator.SetFloat("Velocity",0.15f);
            }
        }

    }
}
