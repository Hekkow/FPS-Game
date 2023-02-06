using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public new string name;
    public string description;
    public string sprite;
    public string functionName;
    public float chance;
    public int maxAmount;
    public float[] amount;
}
