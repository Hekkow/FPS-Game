using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

class EditorScripts
{
    [MenuItem("Mine/Rigidbody/Make Kinematic")]
    static void MakeKinematic()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = true;
            }
        }
    }
    [MenuItem("Mine/Rigidbody/Remove Kinematic")]
    static void RemoveKinematic()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
            }
        }
    }
    [MenuItem("Mine/Collider/Make Trigger")]
    static void MakeTrigger()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            foreach (Collider collider in obj.GetComponents<Collider>())
            {
                collider.isTrigger = true;
            }
        }
    }
    [MenuItem("Mine/Collider/Make Untrigger")]
    static void MakeUntrigger()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            foreach (Collider collider in obj.GetComponents<Collider>())
            {
                collider.isTrigger = false;
            }
        }
    }
    [MenuItem("Mine/Rigidbody/Set Mass")]
    static void SetMass()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.TryGetComponent(out Rigidbody rb))
            {
                rb.mass = 1;
            }
        }
    }
    static void RemoveComponent<T>() where T : Component
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.TryGetComponent(out T dc))
            {
                Object.DestroyImmediate(dc);
            }
        }
    }
    [MenuItem("Mine/Remove Component/Decapitate")]
    static void RemoveDecapitate()
    {
        RemoveComponent<Decapitate>();
    }
}