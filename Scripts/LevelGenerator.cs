using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int segmentCount = 4;
    [SerializeField] private List<LevelSegment> levelSegments = new List<LevelSegment>();

    private List<LevelSegment> currentSegments = new List<LevelSegment>();
    public List<Vector3Int> currentPositions = new List<Vector3Int>();

    [Button]
    public void Generate()
    {
        Clear();
        Vector3Int spawnPoint = Vector3Int.zero;
        currentPositions.Add(spawnPoint);

        Vector2Int exitDirection = Vector2Int.zero;

        LevelSegment beginSegment = Instantiate(levelSegments[Random.Range(0, levelSegments.Count)], spawnPoint, Quaternion.LookRotation(Vector3.forward));
        currentSegments.Add(beginSegment);

        spawnPoint = beginSegment.GetNextPositionFromExit(spawnPoint, beginSegment.GetRandomAvailableExit());
        exitDirection = beginSegment.CurrentExitDirection;

        for (int i = 0; i < segmentCount-1; i++)
        {
            Vector2Int entryDirection = -exitDirection;

            List<LevelSegment> segmentsWithEntry = new List<LevelSegment>();
            for (int j = 0; j < levelSegments.Count; j++)
            {
                if(levelSegments[j].HasValidEntry(entryDirection))
                {
                    segmentsWithEntry.Add(levelSegments[j]);
                }
            }

            //Figure out what position to play the segment so that its next potential spawnpoint doesn't overlap with an existing tile

            LevelSegment segment = Instantiate(segmentsWithEntry[Random.Range(0, segmentsWithEntry.Count)], spawnPoint, Quaternion.LookRotation(Vector3.forward));
            spawnPoint = segment.GetNextPositionFromExit(spawnPoint, beginSegment.GetRandomAvailableExit());
            currentPositions.Add(spawnPoint);
            currentSegments.Add(segment);
        }
    }

    [Button]
    public void Clear()
    {
        for (int i = 0; i < currentSegments.Count; i++)
        {
            Destroy(currentSegments[i].gameObject);
        }
        currentSegments.Clear();
        currentPositions.Clear();
    }
}
