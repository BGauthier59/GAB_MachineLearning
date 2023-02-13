using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;

[XmlRoot("Data")]
public class DataManager : MonoSingleton<DataManager>
{
    public string path;

    XmlSerializer serializer = new XmlSerializer(typeof(Data));
    Encoding encoding = Encoding.UTF8;

    public override void Awake()
    {
        base.Awake();
        SetPath();
    }

    public void Save(List<NeuralNetwork> _nets)
    {
        StreamWriter streamWriter = new StreamWriter(path, false, encoding);
        Data data = new Data
        {
            nets = _nets, 
        };

        serializer.Serialize(streamWriter, data);
        
        streamWriter.Close();
    }

    public Data Load()
    {
        if (File.Exists(path))
        {
            FileStream fileStream = new FileStream(path, FileMode.Open);

            var data = serializer.Deserialize(fileStream) as Data;
            fileStream.Close();
            return data;
        }

        return null;
    }

    void SetPath()
    {
        path = Path.Combine(Application.persistentDataPath, "Data.xml");
    }
}