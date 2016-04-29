using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Assertions;

public class RegisteredPlayerUIView : MonoBehaviour, ISpawnable {

    Image background;
    [SerializeField]
    protected float selectFlashDuration;
    [SerializeField]
    [AutoLink(childPath = "Title")]
    protected Text title;
    [SerializeField]
    [AutoLink(childPath = "CharacterSprite")]
    protected Image CharacterSprite;
    [SerializeField]
    protected float visualsSelectSensitivity;
    AbstractPlayerVisuals spriteSource;
    IEnumerator readyRoutine;
    IEnumerator gameVisualsRoutine;
    Vector2 characterVisualsVector;
    Material myMat;
    LayoutElement layout;
    CanvasGroup canvasGroup;
    AspectRatioFitter aspectRatio;
    float layoutHeight;
    public Vector2 CharacterVisualsVector { get { return characterVisualsVector; } }
    public PlayerRegistration.Registration registration { get; set; }
    public Color playerColor { set { background.color = value; } }
    public string playerName { set { title.text = value; } }

	// Use this for initialization
	void Awake () {
        aspectRatio = GetComponent<AspectRatioFitter>();
        canvasGroup = GetComponent<CanvasGroup>();
        Assert.IsTrue(canvasGroup.alpha == 0);
        layout = GetComponent<LayoutElement>();
        background = GetComponent<Image>();

        myMat = CharacterSprite.material = Instantiate(CharacterSprite.material);
	}

    public void Create()
    {
        Callback.FireForUpdate(() =>
        {
            canvasGroup.alpha = 1;
            layoutHeight = ((RectTransform)transform).rect.height;
            aspectRatio.enabled = false;
            Callback.DoLerp((float l) => layout.preferredHeight = l * layoutHeight, selectFlashDuration, this).FollowedBy(() => aspectRatio.enabled = true, this);
        }, this);
    }

    public void Despawn()
    {
        StopCoroutine(readyRoutine);
        readyRoutine = null;
        if (gameVisualsRoutine != null)
        {
            StopCoroutine(gameVisualsRoutine);
            gameVisualsRoutine = null;
        }
        characterVisualsVector = Vector2.one / 2;

        aspectRatio.enabled = false;
        Callback.DoLerp((float l) => layout.preferredHeight = l * layoutHeight, selectFlashDuration, this, reverse: true).FollowedBy(() =>
        {
            canvasGroup.alpha = 0;
            aspectRatio.enabled = true;
            SimplePool.Despawn(this.gameObject);
        }, this);
    }

    public void UpdateCharacterSprite(int ID)
    {
        CharacterSprite.enabled = true;
        spriteSource = registration.context.charactersData[ID].character.visuals.GetComponent<AbstractPlayerVisuals>();
        UpdateCharacterVisuals(characterVisualsVector = registration.context.charactersData[ID].character.initialVisualsVector);

        Assert.IsNull(readyRoutine);
        readyRoutine = SelectCharacterVisuals();
        StartCoroutine(readyRoutine);

        Assert.IsNull(gameVisualsRoutine);
        gameVisualsRoutine = UpdateCharacterVisuals();
        StartCoroutine(gameVisualsRoutine);
    }

    public void UpdateCharacterVisuals(Vector2 visualSpaceInput)
    {
        Assert.IsTrue(CharacterSprite.enabled);
        Assert.IsNotNull(spriteSource);
        CharacterSprite.sprite = spriteSource.selectionSprite();
        if(spriteSource is IHueShiftableVisuals)
        myMat.SetFloat("_Shift", (spriteSource as IHueShiftableVisuals).visualsToHueVector(visualSpaceInput).x);
    }

    IEnumerator SelectCharacterVisuals()
    {
        yield return null;
        AbstractPlayerInput input = registration.playgroundAvatar.GetComponent<AbstractPlayerInput>();
        if (input != null)
        {
            while (true)
            {
                Vector2 deltaVisuals = input.deltaVisuals();

                //Debug.Log(deltaVisuals);

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

    IEnumerator UpdateCharacterVisuals()
    {
        yield return null;
        IHueShiftableVisuals hueShift = registration.playgroundAvatarVisuals;

        if (hueShift == null)
        {
            gameVisualsRoutine = null;
        }
        else
        {
            Vector2 currentlyDisplayedVector = characterVisualsVector;
            Vector2 previousStoredVector = characterVisualsVector;

            while (true)
            {
                yield return null;

                currentlyDisplayedVector = characterVisualsVector;
                hueShift.shiftAsync = characterVisualsVector;
            }
        }
    }
}
