using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    [SerializeField] InputManager input;
    GameObject fist;
    void Start()
    {
        fist = transform.Find("Camera").Find("Fist").gameObject;
    }
    void Update()
    {
        if (input.punch)
        {
            PunchADude();
        }
    }
    void PunchADude()
    {
        StopCoroutine(StartPunch());
        StartCoroutine(StartPunch());
    }
    IEnumerator StartPunch()
    {
        fist.SetActive(true);
        Helper.AddDamage(fist, 10, 10, false, true);
        yield return new WaitForSeconds(0.1f);
        fist.SetActive(false);

    }
}
