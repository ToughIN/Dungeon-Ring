using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class TitleScreenLoadMenuInputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    [Header("Title Screen Inputs")]
    [SerializeField] private bool deletCharacterSlot = false;

    private void Update()
    {
        if (deletCharacterSlot)
        {
            deletCharacterSlot= false;
            TitleScreenManager.Instance.AttemptToDeleteCharacterSlot();
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.UI.Delete.performed += i => deletCharacterSlot = true; 
        }
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
