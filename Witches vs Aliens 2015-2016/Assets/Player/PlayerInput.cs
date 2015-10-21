using UnityEngine;
using System.Collections;
[RequireComponent(typeof(InputToMovement))]
public class PlayerInput : MonoBehaviour
{
    InputToMovement movement;

    [Tooltip("Name of the axis in the Input Manager")]
    public InputConfiguration bindings;

    // Use this for initialization
    void Awake()
    {
        movement = GetComponent<InputToMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.normalizedInput = new Vector2(Input.GetAxis(bindings.horizontalAxisName), Input.GetAxis(bindings.verticalAxisName)).normalized;
    }
}

[System.Serializable]
public class InputConfiguration
{
    public string verticalAxisName;
    public string horizontalAxisName;

    public InputConfiguration() { }

}