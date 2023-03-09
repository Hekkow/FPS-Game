using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Punch : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject arm;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject fist;

    [Header("Stats")]
    [SerializeField] float punchSpeed;
    [SerializeField] float timeAfterDone;

    Player player;
    bool canPunch = true;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void OnEnable()
    {
        InputManager.playerInput.Player.Punch.performed += LeftPunch;
        InputManager.playerInput.Player.Punch.Enable();
    }
    void LeftPunch(InputAction.CallbackContext obj)
    {
        if (canPunch) StartCoroutine(WaitUntilPunchDone());
    }
    IEnumerator WaitUntilPunchDone()
    {
        canPunch = false;
        if (leftHand.transform.childCount > 0) leftHand.transform.GetChild(0).gameObject.SetActive(false);

        arm.SetActive(true);
        Helper.AddDamage(fist, player.punchDamage, 0, false, true, true);
        animator.Play("Punch", 0, 0);
        animator.speed = punchSpeed;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        yield return new WaitForSeconds(timeAfterDone);
        canPunch = true;
        if (leftHand.transform.childCount > 0) leftHand.transform.GetChild(0).gameObject.SetActive(true);
        arm.SetActive(false);
        Destroy(fist.GetComponent<Damage>());

    }
}
