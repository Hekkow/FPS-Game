using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMarker : MonoBehaviour
{
    GameObject hitMarkerPNG;
    void Start()
    {
        hitMarkerPNG = transform.GetChild(1).gameObject;
    }
    public void Mark()
    {
        StopCoroutine(Hit());
        StartCoroutine(Hit());
    }
    IEnumerator Hit()
    {
        hitMarkerPNG.SetActive(true);
        hitMarkerPNG.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 90));
        yield return new WaitForSeconds(0.2f);
        hitMarkerPNG.SetActive(false);
    }
}
