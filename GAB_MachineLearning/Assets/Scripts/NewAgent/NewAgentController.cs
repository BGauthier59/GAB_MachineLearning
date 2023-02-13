using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NewAgentController : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    public float altitudeInput;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    void FixedUpdate()
    {
        Accelerate();
        Rotate();
    }

    private void Accelerate()
    {
        verticalInput = Sigmoid(verticalInput);
        rb.velocity = transform.forward * (verticalInput * moveSpeed * Time.fixedDeltaTime);
    }

    private static float Sigmoid(float value)
    {
        var k = math.exp(value);
        return k / (1.0f + k);
    }
    
    private void Rotate()
    {
        rb.angularVelocity = (transform.right * altitudeInput + transform.up * horizontalInput) * (rotateSpeed * Time.fixedDeltaTime);
    }

    public void Reset()
    {
        horizontalInput = verticalInput = 0;
    }
}