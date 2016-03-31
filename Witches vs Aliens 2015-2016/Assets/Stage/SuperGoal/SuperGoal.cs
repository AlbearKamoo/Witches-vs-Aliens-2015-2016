using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class SuperGoal : MonoBehaviour {
    Collider2D coll;
    SuperGoal _mirror;
    SpriteRenderer render;
    ParticleSystem vfx;
    PuckFX puckFX;
    SuperGoalSpawner spawner;
    public SuperGoalSpawner Spawner { set { spawner = value; } }
    
    public SuperGoal mirror { set { _mirror = value; } }
    bool _active = false;

    [SerializeField]
    protected float lerpInTime;

    public bool active
    {
        get
        {
            return _active;
        }
        set
        {
            if (value)
            {
                if (!_active)
                {
                    //puckFX.perSideEffectsActive = true;
                    render.enabled = true;
                    rotateParticlesToTransform();
                    vfx.playbackSpeed = 1.5f;
                    Color mainRenderColor = render.color;
                    vfx.Play();
                    Callback.DoLerp((float f) => {
                        render.transform.localScale = new Vector2(f * f * (3 - 2 * f), 1f);
                        render.color = Color.Lerp(Color.white, mainRenderColor, f);
                    }, 1f, this).FollowedBy(() => 
                        {
                            vfx.playbackSpeed = 1f;
                            render.transform.localScale = Vector2.one;
                            coll.enabled = true;
                        }, this);
                     
                }
            }
            else if (_active)
            {
                puckFX.perSideEffectsActive = false;
                coll.enabled = false;
                Color mainRenderColor = render.color;
                vfx.Stop();
                vfx.playbackSpeed = 3;
                Vector2 originalLocalPosition = render.transform.localPosition;
                Callback.DoLerp((float f) =>
                {
                    float tweenedF = 6 * (Mathf.Pow(f, 2) - Mathf.Pow(f, 3)) + 1 - f;
                    render.transform.localScale = new Vector2(tweenedF, 1 + 3 * f);
                    render.transform.localPosition = new Vector2(0f, originalLocalPosition.y + 3 * f * (originalLocalPosition.y - 1));
                    render.color = Color.Lerp(mainRenderColor, Color.white, f);
                }, 1f, this).FollowedBy(() => 
                {
                    Destroy(this.gameObject);

                }, this); 
            }
            _active = value;
        }
    }

    protected virtual void Awake()
    {
        coll = GetComponent<Collider2D>();
        render = GetComponentInChildren<SpriteRenderer>();
        vfx = transform.Find("LightShafts").GetComponent<ParticleSystem>();
        //vfx.Play();
    }

    void Start()
    {
        Callback.FireForUpdate(() => {puckFX = GameObject.FindGameObjectWithTag(Tags.puck).GetComponent<PuckFX>();}, this);
        
    }

	// Use this for initialization
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.CompareTag(Tags.puck))
            return;

        other.collider.transform.position = _mirror.transform.TransformPoint((transform.InverseTransformPoint(other.transform.position)));
        LastBumped bumped = other.collider.GetComponent<LastBumped>();
        spawner.OnSuperGoalScored(bumped);
    }

    void rotateParticlesToTransform()
    {
        vfx.startRotation = Mathf.Deg2Rad * this.transform.eulerAngles.z;
    }
}
