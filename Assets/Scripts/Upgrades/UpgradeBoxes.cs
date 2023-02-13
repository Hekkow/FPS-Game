using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UpgradeBoxes : MonoBehaviour
{
    public AnimationCurve curve;
    float originalY;

    void Start()
    {
        originalY = transform.position.y;
        List<Upgrade> upgrades = UpgradeManager.RandomUpgrade();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject box = transform.GetChild(i).Find("Canvas").GetChild(0).gameObject;
            box.GetComponent<TMP_Text>().text = upgrades[i].name;
            box.GetComponentInParent<PickupUpgrade>().upgrade = upgrades[i];
        }
    }
    // Update is called once per frame
    void Update()
    {
        float y = curve.Evaluate(Time.time);
        transform.position = new Vector3(transform.position.x, y + originalY, transform.position.z);
    }
}