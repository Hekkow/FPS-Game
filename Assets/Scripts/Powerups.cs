using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerups : ScriptableObject
{
    public enum Category
    {
        Mobility,
        Health,
        Any
    }
    public new string name;
    public string description;
    public Category category;
    public string sprite;
    public string functionName;
}
