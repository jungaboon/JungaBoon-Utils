using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Ruler : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    private void OnDrawGizmos()
    {
        if (!pointA && !pointB) return;
        Handles.color = Color.green;
        Vector3 midpoint = (pointA.position + pointB.position) * 0.5f;
        float dist = Vector3.Distance(pointA.position, pointB.position);
        Handles.Label(midpoint + Vector3.up, "Distance = " + dist.ToString());
        Handles.DrawDottedLine(pointA.position, pointB.position, 10f);
    }
}
