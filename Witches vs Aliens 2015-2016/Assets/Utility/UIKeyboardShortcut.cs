using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UIKeyboardShortcut : UIAnyKey
{
    [SerializeField]
    public KeyCode key;

    // Update is called once per frame
    protected override void Update()
    {
        if (Input.GetKeyDown(key))
            SendEvent();

    }
}

