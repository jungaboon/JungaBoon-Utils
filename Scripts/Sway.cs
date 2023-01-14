using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    [Header("Rotational Sway")]
    [SerializeField] private CMF.CameraMouseInput mouseInput;
    [SerializeField] private float swayAmount = 1f;
    [SerializeField] private float smoothScale = 1f;
    [SerializeField] private bool inverseSway;

    [Header("Positional Sway")]
    [SerializeField] private CMF.CharacterKeyboardInput keyboardInput;
    [SerializeField] private float maxAmount = 1f;
    [SerializeField] private float moveSmoothScale = 1f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        RotationalSway();
        PositionalSway();
    }

    private void RotationalSway()
    {
        float x = mouseInput.GetHorizontalCameraInput() * (inverseSway ? -swayAmount : swayAmount);
        float y = mouseInput.GetVerticalCameraInput() * (inverseSway ? -swayAmount : swayAmount);

        Quaternion rotationX = Quaternion.AngleAxis(-y, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(x, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoothScale * Time.deltaTime);
    }

    private void PositionalSway()
    {
        float horizontal = keyboardInput.GetHorizontalMovementInput();
        horizontal = Mathf.Clamp(horizontal, -maxAmount, maxAmount);

        Vector3 lastPos = new Vector3(horizontal, 0f, 0f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, lastPos + startPos, moveSmoothScale * Time.deltaTime);
    }
}
