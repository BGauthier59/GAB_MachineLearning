using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public struct NeuronDisplay
{
    public GameObject go;
    public RectTransform rt;
    private Image image;
    private TMP_Text text;

    public void Init(float xPos, float yPos)
    {
        rt = go.transform as RectTransform;
        //rt = go.GetComponent<RectTransform>();
        image = go.GetComponent<Image>();
        text = go.GetComponentInChildren<TMP_Text>();
        rt.anchoredPosition = new Vector2(xPos, yPos);
    }

    public void Refresh(float value, Color color)
    {
        text.text = value.ToString("F2");
        image.color = color;
    }
}

public struct AxonDisplay
{
    public GameObject go;
    public Image image;
    private RectTransform rt;

    public void Init(RectTransform start, RectTransform end, float thickness, float neuronDiameter)
    {
        rt = go.transform as RectTransform;
        image = go.GetComponent<Image>();
        rt.anchoredPosition = Vector2.Lerp(start.anchoredPosition, end.anchoredPosition, .5f);
        rt.sizeDelta = new Vector2(
            (end.anchoredPosition - start.anchoredPosition).magnitude - neuronDiameter,
            thickness);
        rt.rotation = Quaternion.FromToRotation(rt.right, (end.anchoredPosition - start.anchoredPosition).normalized);
        rt.SetAsFirstSibling();
    }
}

public class NeuralNetworkViewer : MonoBehaviour
{
    [SerializeField] private float layerSpacing = 100;
    [SerializeField] private float neuronVerticalSpacing = 32;
    [SerializeField] private float neuronDiameter = 32;
    [SerializeField] private float axonThickness = 2;
    [SerializeField] private Gradient colorGradient;

    [SerializeField] private GameObject neuronPrefab;
    [SerializeField] private GameObject axonPrefab;
    [SerializeField] private GameObject fitnessPrefab;
    [SerializeField] private RectTransform viewGroup;

    public Agent agent;
    private NeuralNetwork net;

    private NeuronDisplay[][] neurons;
    private AxonDisplay[][][] axons;

    private TMP_Text fitnessDisplay;

    private bool initialized;
    private int maxNeurons;
    private float padding;

    private int x;
    private int y;
    private int z;

    public static NeuralNetworkViewer instance;

    private void Awake()
    {
        instance = this;
    }

    public void Refresh(Agent _agent)
    {
        agent = _agent;
        net = agent.net;
        if (!initialized)
        {
            initialized = true;
            Init();
        }

        RefreshAxons();
    }

    private void Init()
    {
        InitMaxNeurons();
        InitNeurons();
        InitAxons();
        InitFitness();
    }

    private void InitMaxNeurons()
    {
        maxNeurons = 0;
        for (x = 0; x < net.layers.Length; x++)
        {
            if (net.layers[x] > maxNeurons)
            {
                maxNeurons = net.layers[x];
            }
        }
    }

    private void InitNeurons()
    {
        neurons = new NeuronDisplay[net.layers.Length][];

        var maxNeuronIsOdd = maxNeurons % 2;

        for (x = 0; x < net.layers.Length; x++)
        {
            if (net.layers[x] < maxNeurons)
            {
                padding = (maxNeurons - net.layers[x]) * .5f * neuronVerticalSpacing;
                if (net.layers[x] % 2 != maxNeuronIsOdd)
                {
                    padding += neuronVerticalSpacing * .5f;
                }
            }
            else padding = 0;

            neurons[x] = new NeuronDisplay[net.layers[x]];

            for (y = 0; y < net.layers[x]; y++)
            {
                neurons[x][y] = new NeuronDisplay
                {
                    go = Instantiate(neuronPrefab, viewGroup)
                };
                neurons[x][y].Init(x * layerSpacing, -padding - neuronVerticalSpacing * y);
            }
        }
    }

    private void InitAxons()
    {
        axons = new AxonDisplay[net.layers.Length - 1][][];

        for (x = 0; x < net.layers.Length - 1; x++)
        {
            axons[x] = new AxonDisplay[net.layers[x]][];

            for (y = 0; y < net.layers[x]; y++)
            {
                axons[x][y] = new AxonDisplay[net.layers[x + 1]];

                for (z = 0; z < net.layers[x + 1]; z++)
                {
                    axons[x][y][z] = new AxonDisplay
                    {
                        go = Instantiate(axonPrefab, viewGroup)
                    };
                    axons[x][y][z].Init(neurons[x][y].rt, neurons[x + 1][z].rt, axonThickness, neuronDiameter);
                }
            }
        }
    }

    private void InitFitness()
    {
        var fitness = Instantiate(fitnessPrefab, viewGroup);
        fitness.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(net.layers.Length * layerSpacing, -(float)maxNeurons * .5f * neuronVerticalSpacing);
        fitnessDisplay = fitness.GetComponent<TMP_Text>();
    }

    private void RefreshAxons()
    {
        for (x = 0; x < axons.Length; x++)
        {
            for (y = 0; y < axons[x].Length; y++)
            {
                for (z = 0; z < axons[x][y].Length; z++)
                {
                    axons[x][y][z].image.color = colorGradient.Evaluate((net.axons[x][y][z] + 1) * .5f);
                }
            }
        }
    }

    private void Update()
    {
        for (x = 0; x < neurons.Length; x++)
        {
            for (y = 0; y < neurons[x].Length; y++)
            {
                neurons[x][y].Refresh(net.neurons[x][y], colorGradient.Evaluate((net.neurons[x][y] + 1) * .5f));
            }
        }

        fitnessDisplay.text = agent.fitness.ToString("F1");
    }
}