using UnityEngine;

public class CheckpointManager : MonoSingleton<CheckpointManager>
{
    [SerializeField] private Checkpoint[] allCheckpoint;
    public Checkpoint firstCheckpoint;

    private void Init()
    {
        firstCheckpoint = allCheckpoint[0];

        for (int i = 0; i < allCheckpoint.Length - 1; i++)
        {
            allCheckpoint[i].nextCheckpoint = allCheckpoint[i + 1];
        }

        allCheckpoint[^1].nextCheckpoint = firstCheckpoint;
    }

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    public void Reset()
    {
        foreach (var cp in allCheckpoint)
        {
            cp.Refresh();
        }
    }

    public Transform GetClosestCheckpointTransform(Agent agent)
    {
        var closest = allCheckpoint[0];
        var minDist = float.PositiveInfinity;
        foreach (var cp in allCheckpoint)
        {
            //if (cp == agent.lastTakenCheckPoint) continue;
            var sqrDist = (cp.transform.position - agent.transform.position).sqrMagnitude;
            if (sqrDist > minDist) continue;
            minDist = sqrDist;
            closest = cp;
        }

        return closest.ringEntrance;
    }
}