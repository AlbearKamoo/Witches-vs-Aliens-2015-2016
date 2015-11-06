using UnityEngine;
using System.Collections;

public class SetupData : MonoBehaviour {
    public PlayerComponents[] playerComponentPrefabs;
    private static SetupData _self; //there shall only be one
    public static SetupData self { get { return _self; } } //workaround for unity level-loading method order
    void Awake()
    {
        if (_self != null)
            Destroy(this);
        _self = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Destruct()
    {
        _self = null;
        Destroy(this.gameObject);
    }
}
[System.Serializable]
public class PlayerComponents
{
    public CharacterComponents character;
    public InputConfiguration bindings;
    public PlayerComponents() { }
    public PlayerComponents(CharacterComponents character, InputConfiguration bindings)
    {
        this.character = character;
        this.bindings = bindings;
    }
}