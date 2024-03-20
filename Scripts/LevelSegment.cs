using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSegment : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private List<Vector2Int> transitionDirections = new List<Vector2Int>();

    public Vector3Int GetExtents {get{return Vector3Int.RoundToInt(meshRenderer.bounds.extents);}}
    public Vector2Int CurrentExitDirection;

    public bool HasValidEntry(Vector2Int entryDir)
    {
        return transitionDirections.Contains(entryDir);
    }

    // The issue seems to be here
    // The Extents for anything after the initial spawn returns 0 for some reason
    public Vector3Int GetNextPositionFromExit(Vector3Int startPos, Vector2Int exitDir)
    {
        CurrentExitDirection = exitDir;
        Vector3Int addedDir = new Vector3Int(exitDir.x * GetExtents.x * 2, 0, exitDir.y * GetExtents.z * 2);
        Debug.Log($"Extents : {GetExtents} / Added Dir: {addedDir}");
        Vector3Int returnedPos = startPos + addedDir;
        return returnedPos;
    }


    public List<Vector2Int> GetAllAvailableExits(Vector2Int entryDir)
    {
        List<Vector2Int> availableExitDirs = new List<Vector2Int>();
        for (int i = 0; i < transitionDirections.Count; i++)
        {
            if(transitionDirections[i] != entryDir) availableExitDirs.Add(transitionDirections[i]);
        }
        return availableExitDirs;
    }

    public Vector2Int GetRandomAvailableExit()
    {
        Vector2Int chosenExit = transitionDirections[Random.Range(0,transitionDirections.Count)];
        CurrentExitDirection = chosenExit;
        return chosenExit;
    }

    public Vector2Int GetRandomAvailableExit(Vector2Int entryDir)
    {
        // We have to exclude the entry direction from possible exits
        // since we don't want to spawn the next segment in an overlapping spot
        List<Vector2Int> availableExitDirs = new List<Vector2Int>();
        for (int i = 0; i < transitionDirections.Count; i++)
        {
            if(transitionDirections[i] != entryDir) availableExitDirs.Add(transitionDirections[i]);
        }
        Vector2Int chosenExit = availableExitDirs[Random.Range(0,availableExitDirs.Count)];
        CurrentExitDirection = chosenExit;
        Debug.Log("Chosen exit: "+chosenExit);
        return chosenExit;
    }
}
