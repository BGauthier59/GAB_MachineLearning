using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class AgentManager : MonoSingleton<AgentManager>
{
    [SerializeField] private int populationSize = 100;
    [SerializeField] private float trainingDuration = 30;
    [SerializeField] private float mutationRate = 5;
    [SerializeField] private float mutationPower = 5;

    [SerializeField] private Agent agentPrefab;
    [SerializeField] private Transform agentGroup;
    public Transform agentOrigin;

    [SerializeField] private CameraController cameraController;

    private Agent agent;
    private List<Agent> _agents = new List<Agent>();

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text generationCountText;
    private float startingTime;
    private int generationCount;

    public AnimationCurve fitnessOverDistanceMultiplier;
    [SerializeField] private float maxDistanceToReward;
    public float maxSqrDistanceToReward;
    public float distanceWeight;

    [SerializeField] private float addedTimePerGeneration;

    [SerializeField] private AnimationCurve trainingTimeOverFitness;
    [SerializeField] private int firstCopyInNextGeneration;

    private void Start()
    {
        maxSqrDistanceToReward = math.pow(maxDistanceToReward, 2);

        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        StartNewGeneration();
        Focus();
        yield return new WaitForSeconds(trainingDuration);
        //trainingDuration = trainingTimeOverFitness.Evaluate(_agents[0].fitness);
        trainingDuration += addedTimePerGeneration;

        float averageFitness = 0;
        float maxFitness = 0;
        foreach (var a in _agents)
        {
            if (a.fitness > maxFitness) maxFitness = a.fitness;
            averageFitness += a.fitness;
        }

        averageFitness /= _agents.Count;
        Debug.Log($"For Generation {generationCount}, Average fitness is {averageFitness} and Max fitness is {maxFitness}");
        
        StartCoroutine(Loop());
    }

    private void Focus()
    {
        NeuralNetworkViewer.instance.Refresh(_agents[0]);
        cameraController.target = _agents[0].transform;
    }

    private void StartNewGeneration()
    {
        _agents = _agents.OrderByDescending(a => a.fitness).ToList();

        AddOrRemoveAgents();
        Mutate();
        ResetAgents();
        SetMaterials();
        RefreshGenerationCount();
        CheckpointManager.instance.Reset();
        ResetTimer();
    }

    private void Mutate()
    {
        if (_agents.Count % 2 != 0)
        {
            Debug.LogError("Size must be an odd number");
            return;
        }

        _agents[0].name = "First";
        _agents[0].SetFirstMaterial();

        for (int i = 1; i < _agents.Count; i++)
        {
            _agents[i].name = $"Default {i}";
            _agents[i].SetDefaultMaterial();
        }

        var count = 0;
        for (int i = _agents.Count / 2; i < _agents.Count; i++)
        {
            if (count < firstCopyInNextGeneration)
            {
                _agents[i].net.CopyNet(_agents[0].net);
                _agents[i].name = $"First Mutated {i}";
            }
            else if (count > firstCopyInNextGeneration && count < firstCopyInNextGeneration * 2 + 1)
            {
                _agents[i].net.CopyNet(_agents[1].net);
                _agents[i].name = $"Second Mutated {i}";
            }
            else
            {
                _agents[i].net.CopyNet(_agents[i - count * 2].net);
                _agents[i].name = $"Mutated {i}";
            }

            count++;

            _agents[i].net.Mutate(mutationRate, mutationPower);
            _agents[i].SetMutatedMaterial();
        }
    }

    private void ResetAgents()
    {
        for (int i = 0; i < _agents.Count; i++)
        {
            _agents[i].ResetAgent();
        }
    }

    private void SetMaterials()
    {
        return;
        for (int i = 1; i < _agents.Count / 2; i++)
        {
            _agents[i].SetDefaultMaterial();
        }

        _agents[0].SetFirstMaterial();
    }

    void RefreshGenerationCount()
    {
        generationCount++;
        generationCountText.text = generationCount.ToString();
    }

    void ResetTimer()
    {
        startingTime = Time.time;
    }

    private void Update()
    {
        timerText.text = (trainingDuration - (Time.time - startingTime)).ToString("F1");
    }

    private void AddOrRemoveAgents()
    {
        if (_agents.Count != populationSize)
        {
            int dif = populationSize - _agents.Count;

            if (dif > 0)
            {
                for (int i = 0; i < dif; i++)
                {
                    AddAgent();
                }
            }
            else
            {
                for (int i = 0; i < -dif; i++)
                {
                    RemoveAgent();
                }
            }
        }
    }

    private void AddAgent()
    {
        agent = Instantiate(agentPrefab, agentOrigin.position, Quaternion.identity, agentGroup);
        agent.net = new NeuralNetwork(agentPrefab.net.layers);
        _agents.Add(agent);
    }

    private void RemoveAgent()
    {
        Destroy(_agents[^1].gameObject);
        _agents.RemoveAt(_agents.Count - 1);
    }

    #region Buttons

    public void Save()
    {
        List<NeuralNetwork> nets = new List<NeuralNetwork>();

        for (int i = 0; i < _agents.Count; i++)
        {
            nets.Add(_agents[i].net);
        }

        DataManager.instance.Save(nets, generationCount);
    }

    public void Load()
    {
        Data data = DataManager.instance.Load();
        generationCount = data.generation;

        if (data != null)
        {
            for (int i = 0; i < _agents.Count; i++)
            {
                _agents[i].net = data.nets[i];
            }
        }

        End();
    }

    public void End()
    {
        StopAllCoroutines();
        StartCoroutine(Loop());
        ResetTimer();
    }

    #endregion
}