using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class CharacterSelector : MonoBehaviour, IObservable<CharacterSelection>
{

    Observable<CharacterSelection> characterSelectedObservable = new Observable<CharacterSelection>();
    public Observable<CharacterSelection> Observable(IObservable<CharacterSelection> self)
    {
        return characterSelectedObservable;
    }

    int selectedCharacterID = -1;
    public int SelectedCharacterID { get { return selectedCharacterID; } set { selectedCharacterID = value; } }

    public PlayerRegistration.Registration registration { get; set; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (registration.registrationState != PlayerRegistration.RegistrationState.READY && other.CompareTag(Tags.gameController))
        {
            selectedCharacterID = other.GetComponent<CharacterHolder>().characterID;
            characterSelectedObservable.Post(new CharacterSelection(selectedCharacterID));
        }
    }
}

public class CharacterSelection
{
    public readonly int selectedCharacterID;
    public CharacterSelection(int selectedCharacterID)
    {
        this.selectedCharacterID = selectedCharacterID;
    }
}