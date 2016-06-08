using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIEasedFade : MonoBehaviour {

    CanvasGroup canvasGroup;

    float target;

    [SerializeField]
    float currentValue = 1;
    Countdown moveTarget;

    [SerializeField]
    protected float transitionTime;

    [SerializeField]
    protected GameObject defaultSelectedObject;

	// Use this for initialization
	void Awake () {
        canvasGroup = GetComponent<CanvasGroup>();
        moveTarget = new Countdown(moveToTarget, this);
	}

    public void setActive()
    {
        target = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
        moveTarget.Play();
    }

    public void setInactive()
    {
        target = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        moveTarget.Play();
    }

    IEnumerator moveToTarget()
    {
        while (currentValue != target)
        {
            Debug.Log("move");
            currentValue = Mathf.MoveTowards(currentValue, target, Time.deltaTime / transitionTime);
            canvasGroup.alpha = currentValue;
            yield return null;
        }
    }
}
