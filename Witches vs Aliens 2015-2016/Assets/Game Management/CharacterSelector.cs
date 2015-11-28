using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class CharacterSelector : MonoBehaviour {
    CharacterComponents _selectedCharacter;
    public CharacterComponents selectedCharacter { get { return _selectedCharacter; } }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag(Tags.gameController))
            _selectedCharacter = other.GetComponent<CharacterHolder>().character;
    }
}
