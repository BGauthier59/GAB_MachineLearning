using System;
using Unity.Mathematics;
using Random = UnityEngine.Random;

[Serializable]
public class NeuralNetwork
{
    public int[] layers = { 6, 6, 6, 2 };
    public float[][] neurons;
    public float[][][] axons;

    private int x;
    private int y;
    private int yPreviousLayer;
    private int z;

    public NeuralNetwork() { }

    public NeuralNetwork(params int[] layersModel)
    {
        layers = new int[layersModel.Length];

        for (x = 0; x < layersModel.Length; x++)
        {
            layers[x] = layersModel[x];
        }

        InitializeNeuralEngine();
    }

    private void InitializeNeuralEngine()
    {
        #region Neurons

        neurons = new float[layers.Length][];
        for (x = 0; x < layers.Length; x++)
        {
            neurons[x] = new float[layers[x]];
        }

        #endregion

        #region Axons

        axons = new float[layers.Length - 1][][];

        for (x = 0; x < layers.Length - 1; x++)
        {
            axons[x] = new float[layers[x]][];

            for (y = 0; y < layers[x]; y++)
            {
                axons[x][y] = new float[layers[x + 1]];

                for (z = 0; z < layers[x + 1]; z++)
                {
                    axons[x][y][z] = SetAxonValue();
                }
            }
        }

        #endregion
    }

    private float SetAxonValue()
    {
        return Random.Range(-1, 1f);
    }

    private float _value;

    public void FeedForward(params float[] inputs)
    {
        neurons[0] = inputs;

        for (x = 1; x < layers.Length; x++)
        {
            for (y = 0; y < layers[x]; y++)
            {
                _value = 0;
                for (yPreviousLayer = 0; yPreviousLayer < layers[x - 1]; yPreviousLayer++)
                {
                    _value += neurons[x - 1][yPreviousLayer] * axons[x - 1][yPreviousLayer][y];
                }

                neurons[x][y] = math.tanh(_value);
            }
        }
    }

    public void CopyNet(NeuralNetwork model)
    {
        for (x = 0; x < model.axons.Length; x++)
        {
            for (y = 0; y < model.axons[x].Length; y++)
            {
                for (z = 0; z < model.axons[x][y].Length; z++)
                {
                    axons[x][y][z] = model.axons[x][y][z];
                }
            }
        }
    }
    
    public void Mutate(float probability, float power)
    {
        for (x = 0; x < axons.Length; x++)
        {
            for (y = 0; y < axons[x].Length; y++)
            {
                for (z = 0; z < axons[x][y].Length; z++)
                {
                    if (Random.value < probability)
                    {
                        axons[x][y][z] += Random.Range(-power, power);
                    }
                }
            }
        }
    }
}