using UnityEngine;
using System.Collections;

//this particular script has to be placed on the top level of a player via AddComponent, b/c of OnCollisionEnter()

public class Contagion : MonoBehaviour {

    Countdown startCountdown;

	// Use this for initialization
	void Start () {

	}

    public void Initialize()
    {
        startCountdown = Countdown.TimedCountdown(null, 5, this);
    }

}
