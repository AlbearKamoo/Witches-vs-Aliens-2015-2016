using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RegisteredPlayerUIView : MonoBehaviour {

    Image background;
    Text title;
    GameObject LeaveMouseIcon;
    GameObject ReadyMouseIcon;
    GameObject LeaveJoystickIcon;
    GameObject ReadyJoystickIcon;
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
	// Use this for initialization
	void Awake () {
        background = GetComponent<Image>();
        title = transform.Find("Title").GetComponent<Text>();
        LeaveMouseIcon = transform.Find("Leave Tooltip/IconMouse").gameObject;
        ReadyMouseIcon = transform.Find("Ready Tooltip/IconMouse").gameObject;
        LeaveJoystickIcon = transform.Find("Leave Tooltip/IconBumper").gameObject;
        ReadyJoystickIcon = transform.Find("Ready Tooltip/IconBumper").gameObject;
	}

    public void Despawn()
    {
        SimplePool.Despawn(this.gameObject);
    }
}
