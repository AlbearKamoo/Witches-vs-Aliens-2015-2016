using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class CharacterHolder : MonoBehaviour {

    [SerializeField]
    protected float selectFlashDuration;

    public CharacterComponents character;

    public int characterID { get; set; }

    Material myMat;
    Countdown flashCountdown;
    AudioSource audioSource;

    void Awake()
    {
        SpriteRenderer myRend = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = character.selectionSound;
        myMat = myRend.material = Instantiate(myRend.material);
        //myMat.SetFloat(Tags.ShaderParams.cutoff, 0);
        myMat.SetFloat("_Shift", character.initialVisualsVector.x);
        //flashCountdown = new Countdown(Flash, this, playOnAwake : true);
    }

    public void Select()
    {
        //flashCountdown.Restart();
        audioSource.Play();
    }

    IEnumerator Flash()
    {
        return Callback.Routines.DoLerpRoutine((float l) => myMat.SetFloat(Tags.ShaderParams.cutoff, (l + 1)/2), selectFlashDuration, this, reverse: true);
    }
}
[System.Serializable]
public class CharacterComponents
{
    public GameObject basePlayer;
    public GameObject visuals;
    public GameObject movementAbility;
    public GameObject genericAbility;
    public GameObject superAbility;

    public AudioClip selectionSound;

    public Side side;
    public Vector2 initialVisualsVector;
    public CharacterComponents(){}
}