using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSegment : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private List<Vector2> transitionDirections = new List<Vector2>();

    public Vector3 GetExtents {get{return meshRenderer.bounds.extents;}}
    public Vector2 CurrentExitDirection;

    public bool IsValid(Vector3 startPos, Vector2 entryDir, List<Vector3> currentPositions)
    {
        Vector3 nextPos = GetNextPosition(startPos, entryDir);
        for (int i = 0; i < currentPositions.Count; i++)
        {
            if(nextPos == currentPositions[i]) return false;
        }
        return HasEntryDirection(entryDir);
    }
    
    public bool HasEntryDirection(Vector2 entryDirection)
    {
        for (int i = 0; i < transitionDirections.Count; i++)
        {
            if(entryDirection == transitionDirections[i]) return false;
        }
        return true;
    }
    
    public Vector3 GetNextPosition(Vector3 startPos, Vector2 entryDirection)
    {
        Vector2 v_possible = GetPossibleDirections(entryDirection);
        CurrentExitDirection = -v_possible;
        
        Vector3 returnPos = startPos + new Vector3(v_possible.x * GetExtents.x * 2f, 0f, v_possible.y * GetExtents.z * 2f);
        returnPos = Vector3Int.RoundToInt(returnPos);
        return returnPos;
    }
    
    public Vector2 GetPossibleDirections(Vector2 entryDirection)
    {
        List<Vector2> availableDirections = new List<Vector2>();
        for (int i = 0; i < transitionDirections.Count; i++)
        {
            if(transitionDirections[i] != entryDirection) availableDirections.Add(transitionDirections[i]);
        }

        return availableDirections[Random.Range(0,availableDirections.Count)];
    } 
    
    public Vector2 GetPossibleDirections()
    {
        return transitionDirections[Random.Range(0,transitionDirections.Count)];
    }
}
