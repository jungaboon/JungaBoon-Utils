using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ActionBase : MonoBehaviour
{
    public SRC_Action src_action;
    public virtual void ExecuteAction(){}
}

[CreateAssetMenu(fileName = "Action", menuName = "ScriptableObjects/Create New Action")]
public class SRC_Action : ScriptableObject
{
    public AnimationClip actionAnim;
    public float actionValue = 1f;
}
