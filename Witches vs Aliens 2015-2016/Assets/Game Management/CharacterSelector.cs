using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class CharacterSelector : MonoBehaviour
{

    public PlayerRegistration.Registration registration { get; set; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (registration.registrationState != PlayerRegistration.RegistrationState.READY && other.CompareTag(Tags.gameController))
        {
            registration.SelectedCharacterID = other.GetComponent<CharacterHolder>().characterID;
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