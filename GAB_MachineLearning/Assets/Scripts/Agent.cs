using System;
using Unity.Mathematics;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public NeuralNetwork net;
    public float fitness;

    [SerializeField] private NewAgentController controller;
    [SerializeField] private float rayRange = 5f;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Rigidbody rb;

    private float[] inputs;

    private float distanceTraveled;
    [SerializeField] private float totalCheckPointDist;
    public Transform nextCheckpoint;
    [SerializeField] private float nextCheckPointDist;

    //[SerializeField] private Transform closestCheckPoint;
    //public Checkpoint lastTakenCheckPoint;
    //public float maxSqrDistanceReached;

    [SerializeField] private Renderer[] rds;

    [SerializeField] private Material firstMat;
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material mutatedMat;

    public void ResetAgent()
    {
        transform.position = AgentManager.instance.agentOrigin.position;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        inputs = new float[net.layers[0]];
        controller.Reset();
        fitness = 0;

        //maxSqrDistanceReached = 0;
        //closestCheckPoint = null;
        //lastTakenCheckPoint = null;

        totalCheckPointDist = 0;
        nextCheckpoint = CheckpointManager.instance.firstCheckpoint.transform;
    }

    private void FixedUpdate()
    {
        InputUpdate();
        OutputUpdate();
        FitnessUpdate();
    }

    private Vector3 pos;
    private float relativePosY;
    private Vector3 nextCheckPointLocalPos;

    private void InputUpdate()
    {
        pos = transform.position;

        /*
        // Devant
        inputs[0] = RaySensor(pos, transform.forward, 2f);

        // Droite
        inputs[1] = RaySensor(pos, transform.right, 2f);

        // Gauche
        inputs[2] = RaySensor(pos, -transform.right, 2f);

        // Bas
        inputs[3] = RaySensor(pos, -transform.up, 2f);

        // Haut
        inputs[4] = RaySensor(pos, transform.up, 2f);
        
        // Devant droite
        inputs[5] = RaySensor(pos, (transform.forward + transform.right).normalized, 2f);
        
        // Devant gauche
        inputs[6] = RaySensor(pos, (transform.forward - transform.right).normalized, 2f);

*/
        // -1 : trop bas, 0 : bonne altitude, 1 : trop haut

        relativePosY = transform.position.y - nextCheckpoint.position.y;
        relativePosY = (math.tanh(relativePosY) + 1) / 2f;
        //inputs[0] = relativePosY;
        //CheckpointManager.instance.GetClosestCheckpointTransform(this);

        // Devant
        //inputs[0] = RaySensor(pos, transform.forward, 2f);
        
        inputs[0] = GetHorizontalAngle();
        inputs[1] = GetVerticalAngle();
        
        inputs[2] = 1f;
    }

    private RaycastHit hit;

    private float RaySensor(Vector3 origin, Vector3 dir, float length)
    {
        if (Physics.Raycast(origin, dir, out hit, rayRange * length, _layerMask))
        {
            Debug.DrawRay(origin, dir * hit.distance,
                Color.Lerp(Color.red, Color.green, 1 - hit.distance / (rayRange * length)));

            return 1 - hit.distance / (rayRange * length);
        }

        Debug.DrawRay(origin, dir * (length * rayRange), Color.red);
        return 0;
    }

    private float GetHorizontalAngle()
    {
        nextCheckPointLocalPos = transform.InverseTransformPoint(nextCheckpoint.position);
        nextCheckPointLocalPos.y = 0;
        nextCheckPointLocalPos.Normalize();
        var horizontalAngle = Vector3.SignedAngle(Vector3.forward, nextCheckPointLocalPos, Vector3.up) / 180f;

        return horizontalAngle;
    }

    private float GetVerticalAngle()
    {
        nextCheckPointLocalPos = transform.InverseTransformPoint(nextCheckpoint.position);
        nextCheckPointLocalPos.x = 0;
        nextCheckPointLocalPos.Normalize();
        var verticalAngle = Vector3.SignedAngle(Vector3.forward, nextCheckPointLocalPos, Vector3.right) / 180f;

        return verticalAngle;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 3);
    }

    private void OutputUpdate()
    {
        net.FeedForward(inputs);
        controller.verticalInput = net.neurons[^1][0];
        controller.horizontalInput = net.neurons[^1][1];
        controller.altitudeInput = net.neurons[^1][2];
        //controller.rollInput = net.neurons[^1][3];
    }

    private float currentSqrDistance;

    private void FitnessUpdate()
    {
        /*
        currentSqrDistance = (transform.position - AgentManager.instance.agentOrigin.position).sqrMagnitude;

        if (currentSqrDistance > maxSqrDistanceReached)
        {
            fitness += ((currentSqrDistance - maxSqrDistanceReached) *
                        AgentManager.instance.fitnessOverDistanceMultiplier.Evaluate(currentSqrDistance /
                            AgentManager.instance.maxSqrDistanceToReward)) * AgentManager.instance.distanceWeight;
            maxSqrDistanceReached = currentSqrDistance;
        }
        */


        distanceTraveled = totalCheckPointDist +
                           (nextCheckPointDist - (nextCheckpoint.position - transform.position).magnitude);

        if (fitness < distanceTraveled)
        {
            fitness = distanceTraveled;
        }
    }

    public void CheckPointReach(int point, Transform next)
    {
        fitness += point;
        totalCheckPointDist += nextCheckPointDist;
        nextCheckpoint = next;
        nextCheckPointDist = (nextCheckpoint.position - transform.position).magnitude;
    }

    public void SetFirstMaterial()
    {
        foreach (var rd in rds)
        {
            rd.material = firstMat;
        }
    }

    public void SetDefaultMaterial()
    {
        foreach (var rd in rds)
        {
            rd.material = defaultMat;
        }
    }

    public void SetMutatedMaterial()
    {
        foreach (var rd in rds)
        {
            rd.material = mutatedMat;
        }
    }
}