using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NewAgentController : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    public float altitudeInput;
    public float rollInput;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float altitudeSpeed;

    void FixedUpdate()
    {
        //horizontalInput = Input.GetAxis("Horizontal");
        //verticalInput = Input.GetAxis("Vertical");
        //altitudeInput = Input.GetAxis("Altitude");
        //rollInput = Input.GetAxis("Roll");

        Accelerate();
        Rotate();
    }

    private void Accelerate()
    {
        verticalInput = math.clamp(verticalInput, .1f, 1);
        rb.velocity = transform.forward * (verticalInput * moveSpeed * Time.fixedDeltaTime);
    }

    private Vector3 angularVel;
    private void Rotate()
    {
        //rb.angularVelocity = (transform.up * horizontalInput + transform.right * altitudeInput - transform.forward * rollInput).normalized * (rotateSpeed * Time.fixedDeltaTime);
        //transform.localEulerAngles += (transform.up * horizontalInput + transform.right * altitudeInput - transform.forward * rollInput).normalized * (rotateSpeed * Time.fixedDeltaTime);
        //transform.localEulerAngles += (transform.up * horizontalInput).normalized * (rotateSpeed * Time.fixedDeltaTime);
        
        angularVel = Vector3.zero;
        //angularVel += (transform.up * horizontalInput).normalized; //* (rotateSpeed * Time.fixedDeltaTime);
        //angularVel += (transform.right * altitudeInput).normalized; //* (rotateSpeed * Time.fixedDeltaTime);
        //angularVel += (transform.forward * rollInput).normalized;
        //rb.angularVelocity = angularVel.normalized * (rotateSpeed * Time.deltaTime);

        //transform.localEulerAngles += transform.right * (altitudeInput * (altitudeSpeed * Time.fixedDeltaTime));
        //transform.localEulerAngles += (transform.right * altitudeInput).normalized;
        
        
        //

        rb.angularVelocity = (transform.right * altitudeInput).normalized * (rotateSpeed * Time.fixedDeltaTime)
            + (transform.up * horizontalInput).normalized * (rotateSpeed * Time.fixedDeltaTime);
    }

    public void Reset()
    {
        horizontalInput = verticalInput = 0;
    }
}