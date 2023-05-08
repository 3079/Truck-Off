using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[CreateAssetMenu(fileName = "New Curve", menuName = "Scriptable Curve")]
public class SoundHolder : MonoBehaviour
{
    public Sound[] Sounds;

    public void SaveSoundHolder(string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + fileName);
        bf.Serialize(file, Sounds);
        file.Close();
    }

    public void LoadSoundHolder(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
            Sounds = (Sound[])bf.Deserialize(file);
            file.Close();
        }
    }
}
