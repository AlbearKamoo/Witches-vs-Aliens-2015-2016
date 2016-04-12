using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameEndPlayerEntry : MonoBehaviour {

    [SerializeField]
    protected Image playerContainer;

    [SerializeField]
    protected Text numGoalsText;

    int maxNumGoals;
    int numGoals;

    AbstractPlayerVisuals player;

    public void Init(int numGoals, int maxNumGoals, AbstractPlayerVisuals player)
    {
        this.numGoals = numGoals;
        this.maxNumGoals = maxNumGoals;
        this.player = player;
        playerContainer.sprite = player.selectionSprite();

        numGoalsText.text = numGoals.ToString();

        Callback.FireForUpdate(() => //Fire after the aspect ratio component has been initialized
        {
            transform.parent.GetComponent<LayoutElement>().preferredWidth = (transform as RectTransform).rect.width;

            LayoutElement barGraph = GetComponentInChildren<LayoutElement>();
            barGraph.flexibleHeight = 0;
            float targetHeight = ((float)numGoals / (float)maxNumGoals) * (barGraph.transform as RectTransform).rect.height;
            Callback.DoLerp((float l) => barGraph.preferredHeight = targetHeight * l * (3 + l * (-3 + l)), 1, this);
        }, this);
    }
}
