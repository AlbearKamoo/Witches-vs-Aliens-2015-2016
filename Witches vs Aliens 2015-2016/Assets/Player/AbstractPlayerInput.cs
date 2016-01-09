using UnityEngine;
using System.Collections;
[RequireComponent(typeof(InputToAction))]
public abstract class AbstractPlayerInput : MonoBehaviour
{
    protected InputToAction action;


    [Tooltip("Name of the axis in the Input Manager")]
    public InputConfiguration bindings;

    // Use this for initialization
    protected virtual void Start()
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
    public NetworkMode networkMode;

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
    public string movementAbilityAxis;
    [CanBeDefaultOrNull]
    [Tooltip("You can leave this as anything for mouse mode")]
    public string genericAbilityAxis;
    [CanBeDefaultOrNull]
    [Tooltip("You can leave this as anything for mouse mode")]
    public string superAbilityAxis;

    public InputConfiguration() { }

    public enum PlayerInputType
    {
        MOUSE,
        JOYSTICK,
        CRAPAI,
        INTERPOSEAI,
        DEFENSIVEAI,
    }
}

