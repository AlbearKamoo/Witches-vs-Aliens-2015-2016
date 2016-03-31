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
    [SerializeField]
    protected float displayedVisualsUpdateDelay;
    AbstractPlayerVisuals spriteSource;
    IEnumerator readyRoutine;
    IEnumerator gameVisualsRoutine;
    Vector2 characterVisualsVector;
    Material myMat;
    LayoutElement layout;
    float layoutHeight;
    public Vector2 CharacterVisualsVector { get { return characterVisualsVector; } }
    public PlayerRegistration.Registration registration { get; set; }
    public Color playerColor { set { background.color = value; } }
    public string playerName { set { title.text = value; } }

	// Use this for initialization
	void Awake () {
        layout = GetComponent<LayoutElement>();
        layoutHeight = layout.preferredHeight;
        background = GetComponent<Image>();
        characterVisualsVector = new Vector2(Random.value, Random.value);

        myMat = Instantiate(CharacterSprite.material);
        CharacterSprite.material = myMat;
        myMat.SetFloat(Tags.ShaderParams.cutoff, 0);
	}

    public void Create()
    {
        Callback.DoLerp((float l) => layout.preferredHeight = l * layoutHeight, selectFlashDuration, this);
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
        characterVisualsVector = new Vector2(Random.value, Random.value);
        Callback.DoLerp((float l) => layout.preferredHeight = l * layoutHeight, selectFlashDuration, this, reverse : true).FollowedBy(() =>
            SimplePool.Despawn(this.gameObject), this);
    }

    public void UpdateCharacterSprite(int ID)
    {
        CharacterSprite.enabled = true;
        spriteSource = registration.context.charactersData[ID].character.visuals.GetComponent<AbstractPlayerVisuals>();
        UpdateCharacterVisuals(characterVisualsVector = new Vector2(Random.value, Random.value));

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
        CharacterSprite.sprite = spriteSource.selectionSprite(visualSpaceInput);
    }

    IEnumerator SelectCharacterVisuals()
    {
        while (true)
        {
            Vector2 deltaVisuals = Input.GetAxis("Mouse ScrollWheel") * Vector2.right;

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
                yield return new WaitForSeconds(displayedVisualsUpdateDelay);

                if (characterVisualsVector != currentlyDisplayedVector) //if we need to update at all
                {
                    if (characterVisualsVector == previousStoredVector) //if the player has finished choosing
                    {
                        currentlyDisplayedVector = characterVisualsVector;
                        hueShift.shiftAsync = characterVisualsVector;
                    }
                    else
                    {
                        previousStoredVector = characterVisualsVector;
                    }
                }
            }
        }
    }
}
