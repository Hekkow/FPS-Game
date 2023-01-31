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

    public List<string> guns;
    public float maxHealth;
    public float currentHealth;

    //public SaveData(Transform player)
    //{
    //    maxHealth = player.GetComponent<Health>().maxHealth;
    //    //position = new float[3];
    //    //position[0] = player.position.x;
    //    //position[1] = player.position.y;
    //    //position[2] = player.position.z;

    //}
}
