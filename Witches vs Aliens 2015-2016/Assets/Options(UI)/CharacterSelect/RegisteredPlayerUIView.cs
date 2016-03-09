using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Assertions;

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
    [AutoLink(childPath = "Ready Text")]
    protected GameObject ReadyText;
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
    [SerializeField]
    protected float visualsSelectSensitivity;
    AbstractPlayerVisuals spriteSource;
    IEnumerator readyRoutine;
    Vector2 characterVisualsVector;
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
            ReadyText.SetActive(value);
            LeaveTooltip.SetActive(!value);
            ReadyTooltip.SetActive(!value);
            if (value)
            {
                Assert.IsNull(readyRoutine);
                readyRoutine = SelectCharacterVisuals();
                StartCoroutine(readyRoutine);
            }
            else if(readyRoutine != null)
            {
                StopCoroutine(readyRoutine);
                readyRoutine = null;
                UpdateCharacterVisuals(characterVisualsVector = Vector2.zero);
            }
        }
    }
	// Use this for initialization
	void Awake () {
        background = GetComponent<Image>();
        characterVisualsVector = Vector2.zero;
	}

    public void Despawn()
    {
        characterVisualsVector = Vector2.zero;
        SimplePool.Despawn(this.gameObject);
    }

    public void UpdateCharacterSprite(int ID)
    {
        CharacterSprite.enabled = true;
        spriteSource = registration.context.charactersData[ID].character.visuals.GetComponent<AbstractPlayerVisuals>();
        UpdateCharacterVisuals(Vector2.zero);
    }

    public void UpdateCharacterVisuals(Vector2 visualSpaceInput)
    {
        Assert.IsTrue(CharacterSprite.enabled);
        Assert.IsNotNull(spriteSource);
        CharacterSprite.sprite = spriteSource.selectionSprite(visualSpaceInput);
    }

    IEnumerator SelectCharacterVisuals()
    {
        InputToAction action = registration.visuals.GetComponentInParent<InputToAction>();
        while (true)
        {
            Vector2 deltaVisuals = action.normalizedMovementInput;
            if (deltaVisuals != Vector2.zero)
            {
                characterVisualsVector += Time.deltaTime * visualsSelectSensitivity * deltaVisuals;

                //limit to [0,1] range
                characterVisualsVector.x = (characterVisualsVector.x + 1) % 1;
                characterVisualsVector.y = (characterVisualsVector.y + 1) % 1;
                UpdateCharacterVisuals(characterVisualsVector);
            }
            yield return null;
        }
    }
}
