using System;
using System.Collections.Generic;

[Serializable]
public class Data
{
    public List<NeuralNetwork> nets;
    public int generationCount;
}

[Serializable]
public struct DataStruct
{
    public Data data;
}