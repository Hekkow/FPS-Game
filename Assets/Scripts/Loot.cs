using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public string category;
    UI UI;
    void Start()
    {
        UI = GameObject.Find("UI").GetComponent<UI>();
    }
    public void Pickup()
    {
        UI.OpenMenu("upgrade", category);
        Destroy(gameObject);
    }
}
