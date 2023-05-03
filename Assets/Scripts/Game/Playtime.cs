using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Playtime : MonoBehaviour
{
    #if UNITY_EDITOR
    [SerializeField] bool reloadInitialData = false;
    private void OnValidate()
    {
        
        if (reloadInitialData)
        {
            
            reloadInitialData = false;
        }
    }
    #endif
}
