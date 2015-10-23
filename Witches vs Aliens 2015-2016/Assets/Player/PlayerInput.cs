using UnityEngine;
using System.Collections;
[RequireComponent(typeof(InputToAction))]
public class PlayerInput : MonoBehaviour
{
    InputToAction action;

    [Tooltip("Name of the axis in the Input Manager")]
    public InputConfiguration bindings;

    // Use this for initialization
    void Awake()
    {
        action = GetComponent<InputToAction>();
    }

    // Update is called once per frame
    void Update()
    {
        action.normalizedInput = new Vector2(Input.GetAxis(bindings.horizontalAxisName), Input.GetAxis(bindings.verticalAxisName)).normalized;

        if (Input.GetKeyDown(bindings.movementAbilityKey))
            action.FireAbility(AbilityType.MOVEMENT);
        if (Input.GetKeyDown(bindings.superAbilityKey))
            action.FireAbility(AbilityType.SUPER);

        if (Input.GetKeyUp(bindings.movementAbilityKey))
            action.StopFireAbility(AbilityType.MOVEMENT);
        if (Input.GetKeyUp(bindings.superAbilityKey))
            action.StopFireAbility(AbilityType.SUPER);
    }
}

[System.Serializable]
public class InputConfiguration
{
    public string verticalAxisName;
    public string horizontalAxisName;

    public KeyCode movementAbilityKey;
    public KeyCode superAbilityKey;

    public InputConfiguration() { }

}