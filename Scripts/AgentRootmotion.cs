using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentRootmotion : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    [SerializeField] private Transform target;

    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;
    private float chaseTime;

    private void OnDrawGizmos()
    {
        if(target)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, 0.5f);
        }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updatePosition = false;

        StartCoroutine(SynchronizeAnimatorAndAgent());
    }

    private IEnumerator SynchronizeAnimatorAndAgent()
    {
        while(true)
        {
            Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
            worldDeltaPosition.y = 0f;
            float dx = Vector3.Dot(transform.right, worldDeltaPosition);
            float dy = Vector3.Dot(transform.forward, worldDeltaPosition);

            Vector2 deltaPosition = new Vector2(dx, dy);
            float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
            smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

            velocity = smoothDeltaPosition / Time.deltaTime;
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                velocity = Vector2.Lerp(Vector2.zero, velocity, agent.remainingDistance / agent.stoppingDistance);
            }

            bool shouldMove = velocity.sqrMagnitude > 0.5f && agent.remainingDistance > agent.stoppingDistance;
            animator.SetBool("move", shouldMove);
            animator.SetFloat("velocity", velocity.magnitude);
            animator.SetFloat("x", velocity.x);
            animator.SetFloat("y", velocity.y);

            float deltaMagnitude = worldDeltaPosition.normalized.sqrMagnitude;
            if (deltaMagnitude > agent.radius * 0.5f)
            {
                transform.position = Vector3.Lerp(animator.rootPosition, agent.nextPosition, smooth);
            }

            if (chaseTime < 0.15f) chaseTime += Time.deltaTime;
            else
            {
                if (target) agent.SetDestination(target.position);
                chaseTime = 0f;
            }
            yield return null;
        }
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }
}
