using UnityEngine;
using System.Collections;

public class OutOfBoundsCheck : MonoBehaviour {

    Vector2 negativeCorner;
    Vector2 positiveCorner;
	// Use this for initialization
	public void Init (Vector2 negativeCorner, Vector2 positiveCorner) {
        this.negativeCorner = negativeCorner;
        this.positiveCorner = positiveCorner;
        StartCoroutine(CheckBoundsRoutine());
	}

    IEnumerator CheckBoundsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            CheckBounds();
        }
    }

    void CheckBounds()
    {
        Vector2 position = transform.position;
        if (position.x <= negativeCorner.x || position.y <= negativeCorner.y || position.x >= positiveCorner.x || position.y >= positiveCorner.y)
        {
            Debug.LogError("Out of Bounds", this.transform);
            this.transform.position = Vector3.zero;
        }
    }
}
