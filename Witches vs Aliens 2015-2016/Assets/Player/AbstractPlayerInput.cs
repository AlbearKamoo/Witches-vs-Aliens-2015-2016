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

    public abstract bool pressedAccept();

    public abstract bool pressedBack();

    public static bool pressingAccept(InputConfiguration bindings)
    {
        switch (bindings.inputMode)
        {
            case InputConfiguration.PlayerInputType.MOUSE:
                return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);

            case InputConfiguration.PlayerInputType.JOYSTICK:
                //ability axis
                float currentAxisValue = Input.GetAxis(bindings.movementAbilityAxis);

                bool returnValue = false;
                if (currentAxisValue != 0)
                {
                    returnValue = true;
                }

                //now XY axis
                currentAxisValue = Input.GetAxis(bindings.acceptAbilityAxis);

                if (currentAxisValue != 0)
                {
                    returnValue = true;
                }

                return returnValue;
            default:
                return false;
        }
}

    public static bool pressingBack(InputConfiguration bindings)
    {
        switch (bindings.inputMode)
        {
            case InputConfiguration.PlayerInputType.MOUSE:
                return Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1);

            case InputConfiguration.PlayerInputType.JOYSTICK:
                //ability axis
                float currentAxisValue = Input.GetAxis(bindings.genericAbilityAxis);

                bool returnValue = false;
                if (currentAxisValue != 0)
                {
                    returnValue = true;
                }

                //now XY axis
                currentAxisValue = Input.GetAxis(bindings.backAbilityAxis);

                if (currentAxisValue != 0)
                {
                    returnValue = true;
                }

                return returnValue;
            default:
                return false;
        }
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
    [CanBeDefaultOrNull]
    [Tooltip("You can leave this as anything for mouse mode")]
    public string acceptAbilityAxis;
    [CanBeDefaultOrNull]
    [Tooltip("You can leave this as anything for mouse mode")]
    public string backAbilityAxis;

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

