using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Addon
{
    public abstract void Activate();
}
public class Scope : Addon
{
    public override void Activate()
    {
        Debug.Log("TEST");
    }
}
