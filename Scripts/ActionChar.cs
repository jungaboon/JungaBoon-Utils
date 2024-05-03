using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionChar : MonoBehaviour
{
    private GameEventManager gameEventManager;
    [SerializeField] private ActionBase[] actions;

    private void Start() {
        gameEventManager = GameEventManager.Instance;
    }

    public void Interact()
    {
        gameEventManager.CharSelect(actions);
    }
    public void Select()
    {
        Debug.Log($"Selected {name}");
    }
}
