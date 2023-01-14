using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoverPointFinder : MonoBehaviour
{
    [SerializeField] private Collider coll;
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private List<Vector3> viableCoverPoints;

    private void OnDrawGizmos()
    {
        // Find nearest navmesh edges based on renderer's bounding box
        // Find which points are NOT in Player's line of sight
        // Select the one which is closest to agent's current position

        if (coll == null) return;

        float x = coll.bounds.size.x;
        float y = coll.bounds.size.z;

        float startX = x / 2;
        float startY = y / 2;
        Vector3 startPos = coll.bounds.center - new Vector3(startX, coll.bounds.center.y / 2, startY);

        Gizmos.DrawWireSphere(startPos, 0.25f);
        viableCoverPoints.Clear();

        // Checks if the collider type is a box
        if (coll.GetType() != typeof(BoxCollider))
        {
            x += 1;
            y += 1;
        }

        for (int i = 0; i <= x; i++)
        {
            for (int j = 0; j <= y; j++)
            {
                Gizmos.color = Color.red;
                Vector3 gridPos = new Vector3(startPos.x + i, startPos.y, startPos.z + j);

                Vector3 nearestNavpoint = gridPos;
                if(NavMesh.SamplePosition(gridPos, out NavMeshHit sampleHit, 1f, NavMesh.AllAreas))
                {
                    nearestNavpoint = sampleHit.position;
                }

                if (NavMesh.FindClosestEdge(nearestNavpoint, out NavMeshHit navHit, NavMesh.AllAreas))
                {
                    gridPos.y = navHit.position.y;
                    Gizmos.color = Color.yellow;

                    Vector3 dirToTarget = (target.position - gridPos).normalized;
                    Vector3 rayStartPoint = gridPos - dirToTarget * 0.01f;
                    rayStartPoint.y = gridPos.y;

                    if (Physics.Raycast(rayStartPoint, dirToTarget, out RaycastHit hit, 1000f, targetMask))
                    {
                        if (hit.transform != target)
                        {
                            if (!viableCoverPoints.Contains(gridPos)) viableCoverPoints.Add(gridPos);
                            Gizmos.color = Color.green;
                        }
                    }
                }

                Gizmos.DrawCube(gridPos, Vector3.one * 0.5f);


            }
        }
    }
}
