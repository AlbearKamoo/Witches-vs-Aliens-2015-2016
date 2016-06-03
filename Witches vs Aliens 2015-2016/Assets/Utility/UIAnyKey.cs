using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UIAnyKey : MonoBehaviour
{
    // Update is called once per frame
    protected virtual void Update()
    {
        if (Input.anyKeyDown)
            SendEvent();

    }

    protected virtual void SendEvent() //can override this for different functionality
    {
        ExecuteEvents.Execute(this.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler); //hook into the button's code, and cause it to act as if it was pressed
    }
}

