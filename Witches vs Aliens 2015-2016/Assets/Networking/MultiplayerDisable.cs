using UnityEngine;
using System.Collections;

public class MultiplayerDisable : MonoBehaviour {

    [SerializeField]
    protected bool disableWithMultiplayer;

	// Use this for initialization
	void Start () {
        if (disableWithMultiplayer)
        {
            if (NetworkNode.node != null)
            {
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            if (NetworkNode.node == null)
            {
                this.gameObject.SetActive(false);
            }
        }
	}
}
