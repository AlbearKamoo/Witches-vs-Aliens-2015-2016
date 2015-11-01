using UnityEngine;
using System.Collections;
[RequireComponent(typeof(InputToAction))]
public abstract class AbstractPlayerInput : MonoBehaviour
{
    protected InputToAction action;


    [Tooltip("Name of the axis in the Input Manager")]
    public InputConfiguration bindings;

    // Use this for initialization
    void Start()
    {
        action = GetComponent<InputToAction>();
        setInputToActionAimingDelegates();
    }

    protected abstract void setInputToActionAimingDelegates();

    // Update is called once per frame
    void Update()
    {
        updateMovement();

        updateAim();

        checkAbilities();
    }
    protected virtual void updateMovement()
    {
        action.normalizedMovementInput = new Vector2(Input.GetAxis(bindings.horizontalMovementAxisName), Input.GetAxis(bindings.verticalMovementAxisName)).normalized;
    }

    protected abstract void updateAim();

    protected abstract void checkAbilities();
}

[System.Serializable]
public class InputConfiguration
{
    public PlayerInputType inputMode;

    public string verticalMovementAxisName;
    public string horizontalMovementAxisName;

    [CanBeDefaultOrNull]
    [Tooltip("You can leave this as an empty string for mouse mode")]
    public string verticalAimingAxisName;
    [CanBeDefaultOrNull]
    [Tooltip("You can leave this as an empty string for mouse mode")]
    public string horizontalAimingAxisName;

    [CanBeDefaultOrNull]
    [Tooltip("You can leave this as anything for mouse mode")]
    public KeyCode movementAbilityKey;
    [CanBeDefaultOrNull]
    [Tooltip("You can leave this as anything for mouse mode")]
    public KeyCode genericAbilityKey;
    [CanBeDefaultOrNull]
    [Tooltip("You can leave this as anything for mouse mode")]
    public KeyCode superAbilityKey;

    public InputConfiguration() { }

    public enum PlayerInputType
    {
        MOUSE,
        JOYSTICK,
        CRAPAI
    }
}

