using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    [SerializeField] Animator gunAnimator;
    [SerializeField] Player player;

    public void ReloadGun()
    {

        gunAnimator.Play("shoot", 0, 0f);
        
        gunAnimator.speed = player.attackSpeed;
    }
}
