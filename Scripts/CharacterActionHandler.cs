using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionHandler : MonoBehaviour
{
    private GameEventManager gameEventManager;
    [SerializeField] private ActionChar actionChar;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimatorOverrideController animOverride;

    private void Start() {
        gameEventManager = GameEventManager.Instance;
        gameEventManager.onExecuteAction+=SetAction;
    }
    private void OnDestroy() {
        gameEventManager.onExecuteAction-=SetAction;
    }

    private void SetAction(ActionBase actionBase, ActionChar _actionChar)
    {
        if(animator && actionChar == _actionChar)
        {
            animOverride["Action"] = actionBase.src_action.actionAnim;
            animator.runtimeAnimatorController = animOverride;
            animator.SetTrigger("activateAction");
        }
    }
}
