using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NameManager : MonoSingleton<NameManager>
{
    [SerializeField] string[] names;

    private int randomIndex;

    public string GetRandomName(AgentType type)
    {
        randomIndex = Random.Range(0, names.Length);
        string randomName = null;
        randomName += $"{names[randomIndex]}-";
        randomName += type switch
        {
            AgentType.First => "F", 
            AgentType.Second => "S", 
            AgentType.Default => "D", 
            AgentType.Mutated => "M"
        };
        randomName += Random.Range(0, 100).ToString();
        return randomName;
    }
}

public enum AgentType
{
    First,
    Second,
    Default,
    Mutated
}