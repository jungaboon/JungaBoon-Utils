using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraDutch : MonoBehaviour
{
    [SerializeField] private CMF.CharacterKeyboardInput keyboardInput;
    [SerializeField] private CinemachineVirtualCamera virtualCam;

    [SerializeField] private float tiltAmount = 1f;
    [SerializeField] private float tiltSpeed = 1f;

    private void Update()
    {
        float x = keyboardInput.GetHorizontalMovementInput();
        virtualCam.m_Lens.Dutch = Mathf.Lerp(virtualCam.m_Lens.Dutch, -x * tiltAmount, Time.deltaTime * tiltSpeed);
    }
}
