using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Canvas))]
public class AbilityUI : MonoBehaviour {

    Image progress;
    Image background;
    ParticleSystem vfx;

    MovementAbility ability;

    float radius = 0;
    const float particlesPerFillAngle = 400f;

    static Color[] colorLevels = {
                                     Color.clear, 
                                     HSVColor.HSVToRGB(1f/6f, 0.5f, 1f),
                                     HSVColor.HSVToRGB(1f/12f, 0.5f, 1f),
                                     HSVColor.HSVToRGB(0f, 0.5f, 1f),
                                     HSVColor.HSVToRGB(5f/6f, 0.5f, 1f),
                                     HSVColor.HSVToRGB(2f/3f, 0.5f, 1f),
                                     HSVColor.HSVToRGB(1f/2f, 0.5f, 1f),
                                     HSVColor.HSVToRGB(1f/3f, 0.5f, 1f),
                                     Color.white,
                                 };

    float previousFill = 0;
	// Use this for initialization
	void Start () {
        progress = transform.Find("FilledImage").GetComponent<Image>();
        background = transform.Find("BackgroundImage").GetComponent<Image>();
        ability = transform.parent.GetComponentInChildren<MovementAbility>();
        vfx = GetComponent<ParticleSystem>();

        radius = transform.parent.GetComponentInChildren<CircleCollider2D>().radius;

        Vector2 newSizeDelta = new Vector2(2*radius, 2*radius);

        foreach (RectTransform child in transform)
        {
            child.sizeDelta = newSizeDelta;
        }
	}

    void SetFill(float level)
    {
        int floor = Mathf.FloorToInt(level);
        if (floor != Mathf.FloorToInt(previousFill))
        {
            background.color = colorLevels[floor];
            progress.color = colorLevels[floor + 1];
        }
        progress.fillAmount = level - floor;

        Vector2 oldRadial = Quaternion.AngleAxis(360 * previousFill, Vector3.back) * Vector3.up * radius;
        Vector2 newRadial = Quaternion.AngleAxis(360 * level, Vector3.back) * Vector3.up * radius;

        if (level < previousFill && previousFill - level < 0.75f)
        {
            float numParticlesFloat = Mathf.Abs(particlesPerFillAngle * (level - previousFill));

            int numParticles = (int)numParticlesFloat;
            if (numParticlesFloat - numParticles > Random.value)
                numParticles++;

            for (int i = 0; i < numParticles; i++)
            {
                Vector2 radial = Vector3.Slerp(oldRadial, newRadial, Random.value);
                vfx.Emit(Random.value * radial, Vector3.zero, 0.25f, 0.25f, Color.black);
            }
        }

        previousFill = level;
    }
	
	// Update is called once per frame
	void Update () {
        //SetFill(ability.charge);
	}
}
