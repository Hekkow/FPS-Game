using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
