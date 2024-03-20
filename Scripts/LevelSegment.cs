using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSegment : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    public Vector3Int GetExtents {get{return Vector3Int.RoundToInt(meshRenderer.bounds.extents);}}
}
