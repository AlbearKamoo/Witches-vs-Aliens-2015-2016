using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Assertions;

public class RegisteredPlayerUIView : MonoBehaviour {

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
    Vector2 characterVisualsVector;
    Material myMat;
    public Vector2 CharacterVisualsVector { get { return characterVisualsVector; } }
    public PlayerRegistration.Registration registration { get; set; }
    public Color playerColor { set { background.color = value; } }
    public string playerName { set { title.text = value; } }

	// Use this for initialization
	void Awake () {
        background = GetComponent<Image>();
        characterVisualsVector = new Vector2(Random.value, Random.value);

        myMat = Instantiate(CharacterSprite.material);
        CharacterSprite.material = myMat;
        myMat.SetFloat(Tags.ShaderParams.cutoff, 0);
	}

    public void Despawn()
    {
        StopCoroutine(readyRoutine);
        readyRoutine = null;
        characterVisualsVector = new Vector2(Random.value, Random.value);
        SimplePool.Despawn(this.gameObject);
    }

    public void UpdateCharacterSprite(int ID)
    {
        CharacterSprite.enabled = true;
        spriteSource = registration.context.charactersData[ID].character.visuals.GetComponent<AbstractPlayerVisuals>();
        UpdateCharacterVisuals(characterVisualsVector = new Vector2(Random.value, Random.value));

        Assert.IsNull(readyRoutine);
        readyRoutine = SelectCharacterVisuals();
        StartCoroutine(readyRoutine);
    }

    public void UpdateCharacterVisuals(Vector2 visualSpaceInput)
    {
        Assert.IsTrue(CharacterSprite.enabled);
        Assert.IsNotNull(spriteSource);
        CharacterSprite.sprite = spriteSource.selectionSprite(visualSpaceInput);
    }

    IEnumerator SelectCharacterVisuals()
    {
        InputToAction action = registration.selector.GetComponentInParent<InputToAction>();
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
