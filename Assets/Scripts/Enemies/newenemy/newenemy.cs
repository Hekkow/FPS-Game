using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class newenemy : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    bool pathfinding = true;
    [SerializeField] Transform player;
    [SerializeField] float pathfindingInterval;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        //StartCoroutine(Pathfinding());
    }
    private void Update()
    {
        agent.SetDestination(player.position);

    }
    IEnumerator Pathfinding()
    {
        while (pathfinding)
        {
            agent.SetDestination(player.position);
            //if (agent.remainingDistance < maxDistance)
            //{

            //}
            yield return new WaitForSeconds(pathfindingInterval);
        }
    }
}
