using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "Player")
        {
            Destroy(collision.gameObject);
        }
        else
        {
            GameManager.Spawn();
        }
    }
}
