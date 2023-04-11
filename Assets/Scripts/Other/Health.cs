using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public bool alive = true;
    public override string ToString()
    {
        string hashtags = "";
        int curr = Mathf.CeilToInt(health / 10);
        if (curr < 0) curr = 0;
        int max = Mathf.CeilToInt(maxHealth / 10) - curr;
        for (int i = 0; i < curr; i++)
        {
            hashtags += "#";
        }
        for (int i = 0; i < max; i++)
        {
            hashtags += "-";
        }
        return hashtags;
    }
}
