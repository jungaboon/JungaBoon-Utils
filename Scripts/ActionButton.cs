using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    private GameEventManager gameEventManager;
    [SerializeField] private TMP_Text buttonTitle;
    private ActionBase currentAction;
    private ActionChar currentChar;

    private void Start() {
        gameEventManager = GameEventManager.Instance;
    }

    public void SetStatus(bool active)
    {
        gameObject.SetActive(active);
    }
    public void SetActionInfo(ActionBase _action, ActionChar _char)
    {
        currentChar = _char;
        currentAction = _action;
        buttonTitle.text = _action.src_action.name;
    }
    public void ActivateAction()
    {
        gameEventManager.SelectAction();
        gameEventManager.ExecuteAction(currentAction, currentChar);
    }
}
