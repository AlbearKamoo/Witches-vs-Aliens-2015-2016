using UnityEngine;
using System.Collections;

public class HueShiftedAnimation : AnimatedFourWayPlayerVisuals, IHueShiftableVisuals
{
    [SerializeField]
    protected bool flipLeftSprites = false;
    [SerializeField]
    protected bool flipRightSprites = false;

    Vector2 _shift = Vector2.zero;
    public Vector2 shift { set { setHue(_shift.x, value.x); _shift = value; } }

	// Use this for initialization
	protected override void Start () {
        base.Start();
        if (flipLeftSprites)
        {
            flipSpriteArray(leftSprites);
        }
        if (flipRightSprites)
        {
            flipSpriteArray(rightSprites);
        }
	}

    void flipSpriteArray(Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            Rect bounds = sprites[i].rect;
            Texture2D tex = Instantiate(sprites[i].texture);
            Texture2D output = new Texture2D(Mathf.RoundToInt(bounds.width), Mathf.RoundToInt(bounds.height));

            for (int y = Mathf.RoundToInt(bounds.yMin); y < Mathf.RoundToInt(bounds.yMax); y++)
            {
                int xMin = Mathf.RoundToInt(bounds.xMin), xMax = Mathf.RoundToInt(bounds.xMax);
                for (int x = xMin; x < xMax; x++)
                {
                    int xMirror = xMin + (xMax - x) - 1;
                    Color temp = tex.GetPixel(x, y);
                    output.SetPixel(x, y, tex.GetPixel(xMirror, y));
                    output.SetPixel(xMirror, y, temp);
                }
            }
            output.Apply();
            sprites[i] = Sprite.Create(output, Rect.MinMaxRect(0,0,output.width, output.height), Vector2.one / 2, sprites[i].pixelsPerUnit);
        }
    }

    void setHue(float oldHue, float newHue)
    {
        setHueArray(oldHue, newHue, upSprites);
        setHueArray(oldHue, newHue, leftSprites);
        setHueArray(oldHue, newHue, rightSprites);
        setHueArray(oldHue, newHue, downSprites);
        Debug.Log(newHue);
    }

    void setHueArray(float oldHue, float newHue, Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            setHueSprite(oldHue, newHue, ref sprites[i]);
        }
    }

    void setHueSprite(float oldHue, float newHue, ref Sprite sprite)
    {
        Rect bounds = sprite.rect;
        Texture2D tex = Instantiate(sprite.texture);
        Texture2D output = new Texture2D(Mathf.RoundToInt(bounds.width), Mathf.RoundToInt(bounds.height));

        for (int y = Mathf.RoundToInt(bounds.yMin); y < Mathf.RoundToInt(bounds.yMax); y++)
        {
            int xMin = Mathf.RoundToInt(bounds.xMin), xMax = Mathf.RoundToInt(bounds.xMax);
            for (int x = xMin; x < xMax; x++)
            {
                Color temp = tex.GetPixel(x, y);
                float hue, saturation, value;
                HSVColor.RGBToHSV(temp, out hue, out saturation, out value);
                temp = HSVColor.HSVToRGB((hue + (newHue - oldHue)) % 1, saturation, value, temp.a);
                output.SetPixel(x, y, temp);
            }
        }
        output.Apply();
        sprite = Sprite.Create(output, Rect.MinMaxRect(0, 0, output.width, output.height), Vector2.one / 2, sprite.pixelsPerUnit);
    }

    public override Sprite selectionSprite(Vector2 visualSpaceInput)
    {
        Sprite result = base.selectionSprite(visualSpaceInput);
        setHueSprite(0, visualSpaceInput.x, ref result);
        return result;
    }
}

public interface IHueShiftableVisuals
{
    Vector2 shift { set; }
}