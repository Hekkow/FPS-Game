using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject player;
    Health health;
    TMP_Text fpsText;
    TMP_Text healthText;
    TMP_Text speedText;
    float fps;
    float minfps = 10000;
    float maxfps = 0;
    float averagefps;
    List<float> fpsList;

    void Start()
    {
        fpsList = new List<float>();
        health = player.GetComponent<Health>();
        fpsText = gameObject.transform.Find("FPS").GetComponent<TMP_Text>();
        healthText = gameObject.transform.Find("Health").GetComponent<TMP_Text>();
        speedText = gameObject.transform.Find("Speed").GetComponent<TMP_Text>();

        InvokeRepeating("PrintFPS", 1, 0.1f);
        InvokeRepeating("PrintSpeed", 0, 0.1f);
        
    }

    // Update is called once per frame
    void Update()
    {
        PrintHealth();
    }
    void PrintFPS()
    {
        fps = Mathf.Round(1 / Time.unscaledDeltaTime);
        if (fps < minfps) { minfps = fps; }
        if (fps > maxfps) { maxfps = fps; }
        fpsList.Add(fps);
        if (fpsList.Count > 100)
        {
            fpsList.RemoveAt(0);

        }
        averagefps = Mathf.Round(fpsList.Average());

        fpsText.text = fps + " FPS\n" + maxfps + " FPS MAX\n" + minfps + " FPS MIN\n " + averagefps + " FPS AVERAGE";


    }
    void PrintSpeed()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        int speed = Mathf.RoundToInt(new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude);
        speedText.SetText(speed + " SPEEDS");
    }
    void PrintHealth()
    {

        healthText.text = "Health " + Helper.HealthToHashtags(health);
    }
}
