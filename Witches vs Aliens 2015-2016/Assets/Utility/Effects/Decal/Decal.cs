using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
public class Decal : MonoBehaviour {

    [SerializeField]
    protected int pixelWidth;

    [SerializeField]
    protected int pixelHeight;

    [SerializeField]
    protected float unitWidth;

    [SerializeField]
    protected float unitHeight;


    [SerializeField]
    protected GameObject cameraPrefab;

    SpriteRenderer rend;
    Sprite mySprite;

	// Use this for initialization
	void Start () {
        Assert.AreApproximatelyEqual(pixelWidth / unitWidth, pixelHeight / unitHeight);

        rend = GetComponent<SpriteRenderer>();

        GameObject instantiatedCameraGameObject = Instantiate(cameraPrefab);
        Camera instantiatedCameraComponent = instantiatedCameraGameObject.GetComponentInChildren<Camera>();

        //instantiatedCameraComponent.rect = Rect.MinMaxRect(0, 0, Screen.height / Screen.width, 1);
        instantiatedCameraComponent.enabled = false;
        RenderTexture tempTex = RenderTexture.GetTemporary(pixelWidth, pixelHeight);
        instantiatedCameraComponent.targetTexture = tempTex;
        instantiatedCameraComponent.Render();

        Texture2D finishedTexture = new Texture2D(pixelWidth, pixelHeight);

        RenderTexture.active = tempTex;
        finishedTexture.ReadPixels(new Rect(0, 0, pixelWidth, pixelHeight), 0, 0);
        RenderTexture.active = null;
        instantiatedCameraComponent.targetTexture = null;
        RenderTexture.ReleaseTemporary(tempTex);

        finishedTexture.Apply();

        mySprite = Sprite.Create(finishedTexture, Rect.MinMaxRect(0, 0, pixelWidth, pixelHeight), Vector2.one / 2, pixelWidth / unitWidth);
        rend.sprite = mySprite;

        Destroy(instantiatedCameraGameObject);
	}
}
