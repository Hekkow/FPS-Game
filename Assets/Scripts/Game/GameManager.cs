using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject player;
    [SerializeField] Console console;

    [Header("Options")]
    [SerializeField] float timeScale;

    Health playerHealth;

    void Awake()
    {
        Time.timeScale = timeScale;
        playerHealth = player.GetComponent<Health>();
        SaveData data = SaveSystem.ReadSave();
        if (data != null)
        {
            console.commandHistory = data.commandHistory;
            //playerHealth.maxHealth = data.maxHealth;
            //playerHealth.currentHealth = data.currentHealth;
            //player.transform.position = data.position;
            //player.transform.rotation = data.rotation;
            //player.transform.localScale = data.scale;
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
        data.commandHistory = console.commandHistory;
        SaveSystem.WriteSave(JsonUtility.ToJson(data));
    }
    void OnApplicationQuit()
    {
        Save();
    }

}
