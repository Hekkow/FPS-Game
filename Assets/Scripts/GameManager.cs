using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject player;

    public static Vector3 spawnLocation = new Vector3(0, 100, 0);

    Health playerHealth;

    void Start()
    {
        playerHealth = player.GetComponent<Health>();
        SaveData data = SaveSystem.ReadSave();
        if (data != null)
        {
            playerHealth.maxHealth = data.maxHealth;
            playerHealth.currentHealth = data.currentHealth;
            player.transform.position = data.position;
            player.transform.rotation = data.rotation;
            player.transform.localScale = data.scale;
        }
    }
    
    public void Save()
    {
        SaveData data = new SaveData();
        data.maxHealth = playerHealth.maxHealth;
        data.currentHealth = playerHealth.currentHealth;
        data.position = player.transform.position;
        data.rotation = player.transform.rotation;
        data.scale = player.transform.localScale;
        SaveSystem.WriteSave(JsonUtility.ToJson(data));
    }
    public static void Spawn()
    {
        GameObject.Find("Player").transform.position = spawnLocation;
    }
    void OnApplicationQuit()
    {
        Save();
    }

}
