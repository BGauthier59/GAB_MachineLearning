using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private float maxSteerAngle = 42;
    [SerializeField] private float motorForce = 800;

    [SerializeField] private WheelCollider wheelFrontLeftCollider,
        wheelFrontRightCollider,
        wheelRearLeftCollider,
        wheelRearRightCollider;

    [SerializeField] private Transform wheelFrontLeftModel,
        wheelFrontRightModel,
        wheelRearLeftModel,
        wheelRearRightModel;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform centerOfMass;

    public float horizontalInput;
    public float verticalInput;

    private Vector3 pos;
    private Quaternion rot;

    private void Start()
    {
        rb.centerOfMass = centerOfMass.localPosition;
    }

    private void FixedUpdate()
    {
        Steering();
        Accelerate();
        UpdateWheelModels();
    }

    private void Steering()
    {
        wheelFrontLeftCollider.steerAngle = horizontalInput * maxSteerAngle;
        wheelFrontRightCollider.steerAngle = horizontalInput * maxSteerAngle;
    }

    private void Accelerate()
    {
        wheelRearLeftCollider.motorTorque = verticalInput * motorForce;
        wheelRearRightCollider.motorTorque = verticalInput * motorForce;
    }

    private void UpdateWheelModels()
    {
        UpdateWheelModel(wheelFrontLeftCollider, wheelFrontLeftModel);
        UpdateWheelModel(wheelFrontRightCollider, wheelFrontRightModel);
        UpdateWheelModel(wheelRearLeftCollider, wheelRearLeftModel);
        UpdateWheelModel(wheelRearRightCollider, wheelRearRightModel);
    }

    private void UpdateWheelModel(WheelCollider col, Transform tr)
    {
        pos = tr.position;
        rot = tr.rotation;
        
        col.GetWorldPose(out pos, out rot);
        
        tr.position = pos;
        tr.rotation = rot;
    }

    public void Reset()
    {
        horizontalInput = verticalInput = 0;
    }
}