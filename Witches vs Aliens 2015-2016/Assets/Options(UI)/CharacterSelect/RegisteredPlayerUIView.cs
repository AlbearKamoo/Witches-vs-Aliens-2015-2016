using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RegisteredPlayerUIView : MonoBehaviour {

    Image background;
    Text title;
    GameObject LeaveTooltip;
    GameObject ReadyTooltip;
    GameObject LeaveMouseIcon;
    GameObject ReadyMouseIcon;
    GameObject LeaveJoystickIcon;
    GameObject ReadyJoystickIcon;

    GameObject ReadyIcon;
    public Color playerColor { set { background.color = value; } }
    public string playerName { set { title.text = value; } }
    public InputConfiguration.PlayerInputType inputMode
    {
        set
        {
            switch (value)
            {
                case InputConfiguration.PlayerInputType.MOUSE:
                    LeaveMouseIcon.SetActive(true);
                    ReadyMouseIcon.SetActive(true);
                    LeaveJoystickIcon.SetActive(false);
                    ReadyJoystickIcon.SetActive(false);
                    break;
                case InputConfiguration.PlayerInputType.JOYSTICK:
                    LeaveMouseIcon.SetActive(false);
                    ReadyMouseIcon.SetActive(false);
                    LeaveJoystickIcon.SetActive(true);
                    ReadyJoystickIcon.SetActive(true);
                    break;
            }
        }
    }
    public bool ready
    {
        set
        {
            ReadyIcon.SetActive(value);
            LeaveTooltip.SetActive(!value);
            ReadyTooltip.SetActive(!value);
        }
    }
	// Use this for initialization
	void Awake () {
        background = GetComponent<Image>();
        title = transform.Find("Title").GetComponent<Text>();
        LeaveTooltip = transform.Find("Leave Tooltip").gameObject;
        ReadyTooltip = transform.Find("Ready Tooltip").gameObject;
        LeaveMouseIcon = LeaveTooltip.transform.Find("IconMouse").gameObject;
        ReadyMouseIcon = ReadyTooltip.transform.Find("IconMouse").gameObject;
        LeaveJoystickIcon = LeaveTooltip.transform.Find("IconBumper").gameObject;
        ReadyJoystickIcon = ReadyTooltip.transform.Find("IconBumper").gameObject;
        ReadyIcon = transform.Find("ReadyIcon").gameObject;
	}

    public void Despawn()
    {
        SimplePool.Despawn(this.gameObject);
    }
}
