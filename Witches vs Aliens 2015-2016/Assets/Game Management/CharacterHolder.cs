using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class CharacterHolder : MonoBehaviour {

    public CharacterComponents character;

    public int characterID { get; set; }
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
    public CharacterComponents(){}
}