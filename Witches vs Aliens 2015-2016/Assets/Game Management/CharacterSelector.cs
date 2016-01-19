using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class CharacterSelector : MonoBehaviour {
    int selectedCharacterID = -1;
    public int SelectedCharacterID { get { return selectedCharacterID; } }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag(Tags.gameController))
            selectedCharacterID = other.GetComponent<CharacterHolder>().characterID;
    }
}
