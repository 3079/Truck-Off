using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[CreateAssetMenu(fileName = "New Curve", menuName = "Scriptable Curve")]
public class ScriptableCurve : ScriptableObject
{
    public AnimationCurve curve;

    public void SaveCurve(string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + fileName);
        bf.Serialize(file, curve);
        file.Close();
    }

    public void LoadCurve(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
            curve = (AnimationCurve)bf.Deserialize(file);
            file.Close();
        }
    }
}