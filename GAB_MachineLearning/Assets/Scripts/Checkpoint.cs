using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int point;
    private int enteredAgent;
    [SerializeField] private TMP_Text scoreText;
    public Transform ringEntrance;

    public Checkpoint nextCheckpoint;
    
    private void OnTriggerEnter(Collider other)
    {
        var agent = other.GetComponent<Agent>();
        if (!agent) return;
        if (agent.nextCheckpoint != transform) return;

        // Todo - prevent to go backward
        var factor = math.dot(agent.transform.forward, ringEntrance.forward);
        if (factor < 0) return;
        
        enteredAgent++;
        scoreText.text = enteredAgent.ToString();
        agent.CheckPointReach(point, nextCheckpoint.transform);
    }

    public void Refresh()
    {
        enteredAgent = 0;
        scoreText.text = enteredAgent.ToString();
    }
}