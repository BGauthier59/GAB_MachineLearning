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

    [SerializeField] private int firstCopyInNextGeneration;
    private bool autoCalculateLeaderBoard;

    private int generationCount;
    [SerializeField] private TMP_Text generationText;

    private void Start()
    {
        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        StartNewGeneration();
        Focus();
        yield return new WaitForSeconds(trainingDuration);
        StartCoroutine(Loop());
    }

    private void Focus()
    {
        cameraController.target = _agents[0].transform;
    }

    private void StartNewGeneration()
    {
        _agents = _agents.OrderByDescending(a => a.fitness).ToList();
        
        generationCount++;
        generationText.text = $"Generation: {generationCount}";

        AddOrRemoveAgents();
        Mutate();
        ResetAgents();
        CheckpointManager.instance.Reset();
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
            _agents[i].name = NameManager.instance.GetRandomName(AgentType.Default);
            _agents[i].SetDefaultMaterial();
        }

        var count = 0;
        for (int i = _agents.Count / 2; i < _agents.Count; i++)
        {
            if (count < firstCopyInNextGeneration)
            {
                _agents[i].net.CopyNet(_agents[0].net);
                _agents[i].name = NameManager.instance.GetRandomName(AgentType.First);
            }
            else if (count > firstCopyInNextGeneration && count < firstCopyInNextGeneration * 2 + 1)
            {
                _agents[i].net.CopyNet(_agents[1].net);
                _agents[i].name = NameManager.instance.GetRandomName(AgentType.Second);
            }
            else
            {
                _agents[i].net.CopyNet(_agents[i - count * 2].net);
                _agents[i].name = NameManager.instance.GetRandomName(AgentType.Mutated);
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

    private void AddOrRemoveAgents()
    {
        if (_agents.Count != populationSize)
        {
            int dif = populationSize - _agents.Count;

            if (dif > 0)
                for (int i = 0; i < dif; i++)
                    AddAgent();
            else
                for (int i = 0; i < -dif; i++)
                    RemoveAgent();
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

    private Agent[] GetBestAgentsInRun(int count)
    {
        _agents = _agents.OrderByDescending(a => a.fitness).ToList();

        var bests = new List<Agent>();
        for (int i = 0; i < count; i++)
        {
            bests.Add(_agents[i]);
        }

        return bests.ToArray();
    }

    private void Update()
    {
        if (autoCalculateLeaderBoard)
        {
            LeaderBoard();
        }
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
        var data = DataManager.instance.Load();

        if (data != null)
        {
            for (int i = 0; i < _agents.Count; i++)
            {
                _agents[i].net = data.nets[i];
            }
        }

        generationCount = data.generationCount;

        End();
    }

    public void End()
    {
        StopAllCoroutines();
        StartCoroutine(Loop());
    }

    public void LeaderBoard()
    {
        var bests = GetBestAgentsInRun(5);

        foreach (var a in _agents)
        {
            if (bests.Contains(a)) continue;
            if (!a.outline.enabled) continue;
            a.outline.enabled = false;
        }

        for (int i = 1; i < bests.Length + 1; i++)
        {
            if(!bests[i - 1].outline.enabled) bests[i - 1].outline.enabled = true;
            leaderboardTexts[i - 1].text = $"{i} - {bests[i - 1].name} ({bests[i - 1].fitness:F2})";
        }
    }

    [SerializeField] private TMP_Text[] leaderboardTexts;

    public void LeaderBoardToggle()
    {
        autoCalculateLeaderBoard = !autoCalculateLeaderBoard;
    }

    #endregion
}