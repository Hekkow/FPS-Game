using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : MonoBehaviour
{
    public bool grounded = true;
    void OnTriggerEnter(Collider other)
    {
        grounded = true;
    }
    void OnTriggerExit(Collider other)
    {
        grounded = false;
    }
}
