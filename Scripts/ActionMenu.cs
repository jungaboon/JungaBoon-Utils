using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ActionMenu : MonoBehaviour
{
    private GameEventManager gameEventManager;
    [SerializeField] private Image menuBackground;
    [SerializeField] private RectTransform menuAction;
    [SerializeField] private Transform actionHolder;

    private PlayerInputAction playerInput;
    private InputAction rightClick;
    private InputAction leftClick;

    private ActionChar selectedChar;

    private void Start() {
        gameEventManager = GameEventManager.Instance;
        playerInput = new PlayerInputAction();
        rightClick = playerInput.UI.RightClick;
        rightClick.Enable();
        rightClick.started+=_=>CheckForChar();

        leftClick = playerInput.UI.Click;
        leftClick.Enable();
        leftClick.started+=_=>LeftClick();

        gameEventManager.onCharSelect+=OpenMenu;
        gameEventManager.onSelectAction+=CloseMenu;

        menuBackground.gameObject.SetActive(false);
    }
    private void OnDestroy() {
        rightClick.Disable();
        leftClick.Disable();
        gameEventManager.onCharSelect-=OpenMenu;
        gameEventManager.onSelectAction-=CloseMenu;
    }
    private void CheckForChar()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity))
        {
            if(rayHit.transform.TryGetComponent(out ActionChar actionChar))
            {
                selectedChar = actionChar;
                actionChar.Interact();
                return;
            }
        }

        CloseMenu();
    }
    private void OpenMenu(ActionBase[] actions)
    {
        SetMenuHeight(actions);
        SetActionVisibility(actions);
        menuBackground.gameObject.SetActive(true);
        menuBackground.rectTransform.position = Input.mousePosition;
        menuBackground.rectTransform.localScale = Vector2.zero;
        menuBackground.rectTransform.DOScale(Vector2.one, 0.2f).SetEase(Ease.OutQuart);
    }
    private void SetMenuHeight(ActionBase[] actions)
    {
        float h = 0;
        for (int i = 0; i < actions.Length; i++)
        {
            h+=menuAction.sizeDelta.y;
        }
        menuBackground.rectTransform.sizeDelta = new Vector2(menuBackground.rectTransform.sizeDelta.x, h);
    }
    private void SetActionVisibility(ActionBase[] actions)
    {
        for (int i = 0; i < actionHolder.childCount; i++)
        {
            ActionButton actionButton = actionHolder.GetChild(i).GetComponent<ActionButton>();
            if(i<actions.Length) actionButton.SetActionInfo(actions[i], selectedChar);
            actionButton.SetStatus(i<actions.Length);
        }
    }
    private void LeftClick()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Get the pointer event data
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            // Get the raycast results
            List<RaycastResult> results = new List<RaycastResult>(1);
            EventSystem.current.RaycastAll(eventData, results);

            // Check if any of the raycast results are UI buttons
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponent<Button>() != null)
                {
                    // Handle button click
                    return;
                }
            }
        }
        else
        {
            // Check if the raycast is on a target or not
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity))
            {
                if(rayHit.transform.TryGetComponent(out ActionChar actionChar))
                {
                    actionChar.Select();
                }
            }

            CloseMenu();
        }
    }
    private void CloseMenu()
    {
        menuBackground.gameObject.SetActive(false);
    }
}
