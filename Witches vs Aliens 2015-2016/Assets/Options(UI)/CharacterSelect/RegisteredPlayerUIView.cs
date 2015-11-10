using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RegisteredPlayerUIView : MonoBehaviour {

    Image background;
    Text title;
    public Color playerColor { set { background.color = value; } }
    public string playerName { set { title.text = value; } }
	// Use this for initialization
	void Awake () {
        background = GetComponent<Image>();
        title = transform.Find("Title").GetComponent<Text>();
	}

    public void Despawn()
    {
        SimplePool.Despawn(this.gameObject);
    }
}
