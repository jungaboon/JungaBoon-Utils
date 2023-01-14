using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Grid : MonoBehaviour
{
	[SerializeField] private bool useCircle;
	
	[HideInInspector] public int x;
	[HideInInspector] public int y;
	[HideInInspector] public float radius;
	private float gridCubeSize = 0.25f;
	
	[Space]
	[SerializeField] private Transform target;
	[SerializeField] private LayerMask targetMask;
	[SerializeField] private LayerMask gridposMask;
	[SerializeField] private Gradient gridColors;
	
	[Space]
	[SerializeField] private float raycastHeight = 10f;
	[SerializeField] private float minDistToTarget = 2f; // Testing purposes only -- Point must be at least this far from the target to be valid
	
	private Vector3 GetClosestPointOnGrid(Transform _target, List<Vector3> _points)
	{
		float[] dist = new float[_points.Count];
		
		for (int i = 0; i < _points.Count; i++) 
		{
			float d = Vector3.Distance(_points[i], target.position);
			dist[i] = d;
		}
		
		float smallestDist = dist.Min();
		int ind = Array.IndexOf(dist, smallestDist);
		
		return _points[ind];
	}
	
	private Vector3 GetFarthestPointOnGrid(Transform _target, List<Vector3> _points)
	{
		float[] dist = new float[_points.Count];
		
		for (int i = 0; i < _points.Count; i++) 
		{
			float d = Vector3.Distance(_points[i], target.position);
			dist[i] = d;
		}
		
		float largestDist = dist.Max();
		int ind = Array.IndexOf(dist, largestDist);
		
		return _points[ind];
	}
	
	private Vector3 GetNearestNavEdge(Vector3 _pos)
	{
		Vector3 nearestNavmeshEdge; 
				
		if(NavMesh.FindClosestEdge(_pos, out NavMeshHit navHit, NavMesh.AllAreas))
		{
			nearestNavmeshEdge = navHit.position;
		}
		else
		{
			nearestNavmeshEdge = _pos;
		}
		
		return nearestNavmeshEdge;
	}
	
	protected void OnDrawGizmos()
	{
		List<Vector3> viableGridPositions = new List<Vector3>();
		viableGridPositions.Clear();
		
		
		if(useCircle)
		{
			Handles.color = Color.green;
			Handles.DrawWireDisc(transform.position, Vector3.up, radius, 3f);
			
			float numOfGrids = Mathf.Ceil(radius * 2);
			Vector3 startPos = transform.position - new Vector3(radius, 0f, radius);
			
			
			for (int i = 0; i <= numOfGrids; i++) 
			{
				for (int j = 0; j <= numOfGrids; j++) 
				{
					float posValue = 0f;
					
					Vector3 gridPos = new Vector3(startPos.x + i, transform.position.y, startPos.z + j);
					
					if(Physics.Raycast(gridPos + Vector3.up * raycastHeight, Vector3.down, out RaycastHit firstRayHit, 100f, gridposMask))
					{
						gridPos.y = firstRayHit.point.y + (gridCubeSize / 2);
					}
					
					float gridDistToCenter = Vector3.Distance(gridPos, transform.position);
					
					if(gridDistToCenter <= radius)
					{
						if(Physics.Raycast(gridPos, target.position - gridPos, out RaycastHit rayHit, 100f, targetMask))
						{
							if(rayHit.transform == target) 
							{
								posValue += 0.33f;
								
								if(NavMesh.SamplePosition(gridPos, out NavMeshHit navHit, gridCubeSize, NavMesh.AllAreas)) 
								{
									posValue += 0.33f;
									
									// Useful for if the agent needs to keep a certain distance away from target
									float distToTarget = Vector3.Distance(target.position, gridPos);
									if(distToTarget >= minDistToTarget) 
									{
										posValue += 0.34f;
										if(!viableGridPositions.Contains(gridPos)) viableGridPositions.Add(gridPos);
									}
								}
							}
						}
						
						// Creates visualization of the grids
						Gizmos.color = gridColors.Evaluate(posValue);
						Gizmos.DrawCube(gridPos, Vector3.one * gridCubeSize);
						Handles.color = Gizmos.color;
						Handles.Label(gridPos + Vector3.up * 0.25f, posValue.ToString());
					}
				}
			}
		}
		else
		{
			float startX = x / 2;
			float startY = y / 2;
			Vector3 startPos = transform.position - new Vector3(startX, 0f, startY);
			
			for (int i = 0; i < x; i++) 
			{
				for (int j = 0; j < y; j++) 
				{
					float posValue = 0f;
					Vector3 gridPos = new Vector3(startPos.x + i, transform.position.y, startPos.z + j);
					
					if(Physics.Raycast(gridPos + Vector3.up * raycastHeight, Vector3.down, out RaycastHit firstRayHit, 100f, gridposMask))
					{
						gridPos.y = firstRayHit.point.y + (gridCubeSize / 2);
					}
					
					float gridDistToCenter = Vector3.Distance(gridPos, transform.position);
					
					
					if(Physics.Raycast(gridPos, target.position - gridPos, out RaycastHit rayHit, 100f, targetMask))
					{
						if(rayHit.transform == target) 
						{
							posValue += 0.33f;
								
							if(NavMesh.SamplePosition(gridPos, out NavMeshHit navHit, gridCubeSize, targetMask)) 
							{
								posValue += 0.33f;
									
								// Useful for if the agent needs to keep a certain distance away from target
								float distToTarget = Vector3.Distance(target.position, gridPos);
								if(distToTarget >= minDistToTarget) 
								{
									posValue += 0.34f;
									if(!viableGridPositions.Contains(gridPos)) viableGridPositions.Add(gridPos);
								}
							}
						}
					}
						
					Gizmos.color = gridColors.Evaluate(posValue);
					Gizmos.DrawCube(gridPos, Vector3.one * gridCubeSize);
					Handles.color = Gizmos.color;
					Handles.Label(gridPos + Vector3.up * 0.25f, posValue.ToString());

				}
			}
		}
		
		// Calculate which grid position distance to target
		if(viableGridPositions.Count > 0)
		{
			Vector3 closestPos = GetFarthestPointOnGrid(target, viableGridPositions);
				
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(GetNearestNavEdge(closestPos), 0.5f);
		}
	}
	
	#if UNITY_EDITOR
	[CustomEditor(typeof(Grid))]
	public class GridEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var grid = (Grid)target;
			if(grid == null) return;
			Undo.RecordObject(grid, "Updated Grid");
			
			if(grid.useCircle) grid.radius = EditorGUILayout.FloatField("Radius", grid.radius);
			else
			{
				grid.x = EditorGUILayout.IntField("X", grid.x);
				grid.y = EditorGUILayout.IntField("Y", grid.y);
			}
			
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
	}
	#endif
}
