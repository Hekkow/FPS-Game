using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Vector3 velocity;
    public List<string> commandHistory;
    public float maxHealth;
    public float currentHealth;
}
public static class SaveSystem
{
    static string path = Application.persistentDataPath + "/save.file";
    public static void WriteSave(string data)
    {
        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine(data);
        writer.Close();
    }
    public static SaveData ReadSave()
    {
        if (File.Exists(path))
        {
            StreamReader reader = new StreamReader(path);
            SaveData data = JsonUtility.FromJson<SaveData>(reader.ReadToEnd());
            reader.Close();
            return data;
        } else
        {
            Debug.Log("Save file not found");
            return null;
        }
    }
}
