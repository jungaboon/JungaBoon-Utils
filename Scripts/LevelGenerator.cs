using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int segmentCount = 4;
    [SerializeField] private LevelSegment[] levelSegments;

    private List<LevelSegment> currentSegments = new List<LevelSegment>();
    public List<Vector3> currentPositions = new List<Vector3>();

    [Button]
    public void Generate()
    {
        Clear();
        Vector3 spawnPoint = Vector3.zero;
        currentPositions.Add(spawnPoint);

        Vector2 exitDirection = Vector2.zero;

        LevelSegment beginSegment = Instantiate(levelSegments[Random.Range(0, levelSegments.Length)], spawnPoint, Quaternion.LookRotation(Vector3.forward));
        currentSegments.Add(beginSegment);

        spawnPoint = beginSegment.GetNextPosition(spawnPoint, Vector2.zero);
        exitDirection = beginSegment.CurrentExitDirection;

        for (int i = 0; i <= segmentCount; i++)
        {
            Vector2 entryDirection = Vector2.zero;
            entryDirection = -exitDirection;

            int attempts = 100;

            List<LevelSegment> segmentsWithEntry = new List<LevelSegment>();
            for (int j = 0; j < levelSegments.Length; j++)
            {
                if(levelSegments[j].HasEntryDirection(entryDirection)) segmentsWithEntry.Add(levelSegments[j]);
            }

            LevelSegment valid_s = segmentsWithEntry[Random.Range(0,segmentsWithEntry.Count
            )];

            while(attempts > 0 && !valid_s.IsValid(spawnPoint, entryDirection, currentPositions))
            {
                valid_s = levelSegments[Random.Range(0,levelSegments.Length)];
                attempts --;
            }

            LevelSegment segment = Instantiate(valid_s, spawnPoint, Quaternion.LookRotation(Vector3.forward));

            currentPositions.Add(spawnPoint);
            currentSegments.Add(segment);

            spawnPoint = segment.GetNextPosition(spawnPoint, entryDirection);
            exitDirection = segment.CurrentExitDirection;
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
