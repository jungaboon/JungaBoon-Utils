using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LevelSegmentDirection
{
    public string name;
    public GameObject[] segments;
    public List<Vector2Int> validDirections;
}
public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int segmentCount = 4;
    [SerializeField] private Vector3Int extents = new Vector3Int();
    [SerializeField] private LevelSegmentDirection[] startSegments;
    [SerializeField] private LevelSegmentDirection[] levelSegmentDirections;
    [SerializeField] private List<Vector2Int> possibleDirections;
    private List<GameObject> currentSegments = new List<GameObject>();

    public void Generate()
    {
        Clear();
        // Initial spawn
        Vector3Int spawnPos = Vector3Int.zero;
        int randomStart = Random.Range(0,startSegments.Length);
        GameObject startSegment = Instantiate(startSegments[randomStart].segments[Random.Range(0,startSegments[randomStart].segments.Length)], spawnPos, Quaternion.identity);
        currentSegments.Add(startSegment);

        // Choose next spawn direction
        int indexOfNextDirection = possibleDirections.IndexOf(possibleDirections[randomStart]);
        Vector2Int transitionDirection = possibleDirections[indexOfNextDirection];
        spawnPos += GetNextPos(transitionDirection, extents);

        for (int i = 0; i < segmentCount; i++)
        {
            List<Vector2Int> nextPossibleDirections = new List<Vector2Int>();
            for (int j = 0; j < possibleDirections.Count; j++)
            {
                if(possibleDirections[j] != -transitionDirection) 
                {
                    nextPossibleDirections.Add(possibleDirections[j]);
                }
            }

            Vector2Int nextDirection = nextPossibleDirections[Random.Range(0,nextPossibleDirections.Count)];
            List<Vector2Int> prevAndNextDirections = new List<Vector2Int>();
            prevAndNextDirections.Add(-transitionDirection);
            prevAndNextDirections.Add(nextDirection);

            Debug.DrawRay(spawnPos, new Vector3(prevAndNextDirections[0].x, 0f, prevAndNextDirections[0].y) * 5f, Color.red, 5f);
            Debug.DrawRay(spawnPos, new Vector3(prevAndNextDirections[1].x, 0f, prevAndNextDirections[1].y) * 5f, Color.green, 5f);
            Debug.Log($"PrevDir: {prevAndNextDirections[0]} / NextDir: {prevAndNextDirections[1]}");

            GameObject s = new GameObject();

            // Get a segment that fits the criteria of having the same exit direction and same next direction
            for (int k = 0; k < levelSegmentDirections.Length; k++)
            {
                if(ContainsList(levelSegmentDirections[k].validDirections, prevAndNextDirections))
                {
                    int index = Random.Range(0, levelSegmentDirections[k].segments.Length);
                    s = levelSegmentDirections[k].segments[index];
                    break;
                }
            }

            GameObject segment = Instantiate(s, spawnPos, Quaternion.identity);
            currentSegments.Add(segment);
            
            spawnPos += GetNextPos(nextDirection, extents);
            transitionDirection = nextDirection;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < currentSegments.Count; i++)
        {
            Destroy(currentSegments[i].gameObject);
        }
        currentSegments.Clear();
    }

    private Vector3Int GetNextPos(Vector2Int direction, Vector3Int inputExtents)
    {
        return new Vector3Int(direction.x * inputExtents.x, 0, direction.y * inputExtents.z);
    }

    private bool ContainsList(List<Vector2Int> allDirections, List<Vector2Int> directionsToCheck)
    {
        for (int i = 0; i < directionsToCheck.Count; i++)
        {
            if(!allDirections.Contains(directionsToCheck[i])) return false;
        }
        return true;
    }
}
