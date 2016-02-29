using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RegisteredPlayerUIView : MonoBehaviour {

    Image background;
    [SerializeField]
    [AutoLink(childPath = "Title")]
    protected Text title;
    [SerializeField]
    [AutoLink(childPath = "Leave Tooltip")]
    protected GameObject LeaveTooltip;
    [SerializeField]
    [AutoLink(childPath = "Ready Tooltip")]
    protected GameObject ReadyTooltip;
    [SerializeField]
    [AutoLink(childPath = "Leave Tooltip/IconMouse")]
    protected GameObject LeaveMouseIcon;
    [SerializeField]
    [AutoLink(childPath = "Ready Tooltip/IconMouse")]
    protected GameObject ReadyMouseIcon;
    [SerializeField]
    [AutoLink(childPath = "Leave Tooltip/IconBumper")]
    protected GameObject LeaveJoystickIcon;
    [SerializeField]
    [AutoLink(childPath = "Ready Tooltip/IconBumper")]
    protected GameObject ReadyJoystickIcon;
    [SerializeField]
    [AutoLink(childPath = "ReadyIcon")]
    protected GameObject ReadyIcon;
    [SerializeField]
    [AutoLink(childPath = "CharacterSprite")]
    protected Image CharacterSprite;
    public PlayerRegistration.Registration registration { get; set; }
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
	}

    public void Despawn()
    {
        SimplePool.Despawn(this.gameObject);
    }

    public void UpdateCharacterSprite(int ID)
    {
        CharacterSprite.enabled = true;
        CharacterSprite.sprite = registration.context.charactersData[ID].character.visuals.GetComponent<AbstractPlayerVisuals>().selectionSprite;
    }
}
