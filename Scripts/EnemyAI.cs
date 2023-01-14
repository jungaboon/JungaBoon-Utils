using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private GameObject player;
    private Animator animator;
    private NavMeshAgent agent;

    [Header("IK Aim")]
    [SerializeField] private Transform aimTarget;

    private float chaseTime;

    private int _idleType = Animator.StringToHash("idleType");
    private int _runType = Animator.StringToHash("runType");
    private int _x = Animator.StringToHash("x");
    private int _xInv = Animator.StringToHash("xInv");
    private int _y = Animator.StringToHash("y");
    private int _velocity = Animator.StringToHash("velocity");
    private int _playerDistance = Animator.StringToHash("playerDistance");

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        // Set Animator values
        animator.SetFloat(_idleType, Random.Range(0f, 3f));
        animator.SetFloat(_runType, Random.Range(0f, 3f));

        StartCoroutine(MoveAimTargetToPlayer());
    }

    private void Update()
    {
        if (chaseTime < 0.3f) chaseTime += Time.deltaTime;
        else
        {
            if(agent.isActiveAndEnabled) agent.SetDestination(player.transform.position);
            chaseTime = 0f;
        }
    }


    private IEnumerator MoveAimTargetToPlayer()
    {
        while(true)
        {
            aimTarget.position = Vector3.Lerp(aimTarget.position, player.transform.position + Vector3.up * 1.75f, 0.5f);

            Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
            float playerDistance = Vector3.Distance(player.transform.position, transform.position);

            // When at 1, this means the Player is to the right of the zombie
            float x = Vector3.Dot(transform.right, dirToPlayer);

            // When at 1, this means the Player is to the left of the zombie
            float xInv = Vector3.Dot(-transform.right, dirToPlayer);

            // When at 1, this means the Player is to the front of the zombie
            float y = Vector3.Dot(transform.forward, dirToPlayer);

            animator.SetFloat(_x, x);
            animator.SetFloat(_xInv, xInv);
            animator.SetFloat(_y, y);
            animator.SetFloat(_playerDistance, playerDistance);

            yield return null;
        }
    }
}
