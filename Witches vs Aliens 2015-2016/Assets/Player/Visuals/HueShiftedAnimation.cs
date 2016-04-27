using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class HueShiftedAnimation : AnimatedFourWayPlayerVisuals, IHueShiftableVisuals
{
    [SerializeField]
    protected bool flipLeftSprites = false;
    [SerializeField]
    protected bool flipRightSprites = false;

    [SerializeField]
    protected float hueMin;

    [SerializeField]
    protected float hueRange;

    [SerializeField]
    protected float numShades;

    Vector2 _shift;
    public Vector2 shift
    {
        get { return _shift; }
        set
        {
            Vector2 hueSpaceShiftVector = visualsToHueVector(_shift);
            Vector2 hueSpaceValueVector = visualsToHueVector(value);
            setHue(hueSpaceShiftVector.x, hueSpaceValueVector.x);
            _shift = value;
        }
    }

    public Vector2 shiftAsync
    {
        set
        {
            Vector2 hueSpaceShiftVector = visualsToHueVector(_shift);
            Vector2 hueSpaceValueVector = visualsToHueVector(value);
            StartCoroutine(setHueAsync(hueSpaceShiftVector.x, hueSpaceValueVector.x));
            _shift = value;
        }
    }

    Vector2 selectionShift = Vector2.zero;
    Sprite selectSprite;

	// Use this for initialization
    protected override void Awake()
    {
        base.Awake();

        duplicateSpriteArray(upSprites);
        duplicateSpriteArray(leftSprites);
        duplicateSpriteArray(rightSprites);
        duplicateSpriteArray(downSprites);

        if (flipLeftSprites)
        {
            flipSpriteArray(leftSprites);
        }
        if (flipRightSprites)
        {
            flipSpriteArray(rightSprites);
        }

        if (selectSprite == null)
        {
            selectSprite = duplicateSprite(selectionSprite());
            selectionShift = visualsToHueVector(Vector2.zero);
            setHueSprite(0, selectionShift.x, selectSprite);
        }

        _shift = visualsToHueVector(Vector2.zero);
        //setHue(0, _shift.x);
        StartCoroutine(setHueAsync(0, _shift.x));
    }

    void duplicateSpriteArray(Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = duplicateSprite(sprites[i]);
        }
        
    }

    Sprite duplicateSprite(Sprite original)
    {
        Rect bounds = original.rect;
        Texture2D tex = original.texture;
        Texture2D output = new Texture2D(Mathf.RoundToInt(bounds.width), Mathf.RoundToInt(bounds.height));
        int x = Mathf.RoundToInt(bounds.xMin);
        int y = Mathf.RoundToInt(bounds.yMin);
        int xBlockSize = Mathf.RoundToInt(bounds.width);
        int yBlockSize = Mathf.RoundToInt(bounds.height);
        output.SetPixels(0, 0, xBlockSize, yBlockSize, tex.GetPixels(x, y, xBlockSize, yBlockSize));
        output.Apply();
        return Sprite.Create(output, Rect.MinMaxRect(0, 0, output.width, output.height), Vector2.one / 2, original.pixelsPerUnit);
    }

    void flipSpriteArray(Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            Rect bounds = sprites[i].rect;
            Texture2D tex = sprites[i].texture;

            for (int y = Mathf.RoundToInt(bounds.yMin); y < Mathf.RoundToInt(bounds.yMax); y++)
            {
                int xMin = Mathf.RoundToInt(bounds.xMin), xMax = Mathf.RoundToInt(bounds.xMax);
                int xMiddle = Mathf.RoundToInt((bounds.xMax + bounds.xMin) / 2);
                for (int x = xMin; x < xMiddle; x++)
                {
                    int xMirror = xMin + (xMax - x) - 1;
                    Color temp1 = tex.GetPixel(x, y);
                    Color temp2 = tex.GetPixel(xMirror, y);
                    tex.SetPixel(x, y, temp2);
                    tex.SetPixel(xMirror, y, temp1);
                }
            }
            tex.Apply();
        }
    }

    public Vector2 visualsToHueVector(Vector2 characterVisualsVector)
    {
        Vector2 result = characterVisualsVector;

        //have the vector snap to (n = numShades) spots
        result *= numShades;
        result.x = Mathf.Floor(result.x);
        result.y = Mathf.Floor(result.y);
        result /= numShades;

        //move the vector from [0, 1] to [hueMin, hueMin + hueRange] space
        result *= hueRange;

        result.x = result.x + hueMin;
        result.y = result.y + hueMin;
        
        result.x = result.x % 1;
        result.y = result.y % 1;

        return result;
    }

    void setHue(float oldHue, float newHue)
    {
        setHueArray(oldHue, newHue, downSprites);
        setHueArray(oldHue, newHue, upSprites);
        setHueArray(oldHue, newHue, leftSprites);
        setHueArray(oldHue, newHue, rightSprites);
    }

    IEnumerator setHueAsync(float oldHue, float newHue) //use coroutines to not cause a single-frame lag spike
    {
        yield return StartCoroutine(setHueArrayAsync(oldHue, newHue, downSprites));
        yield return StartCoroutine(setHueArrayAsync(oldHue, newHue, upSprites));
        yield return StartCoroutine(setHueArrayAsync(oldHue, newHue, leftSprites));
        yield return StartCoroutine(setHueArrayAsync(oldHue, newHue, rightSprites));
    }

    void setHueArray(float oldHue, float newHue, Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            setHueSprite(oldHue, newHue, sprites[i]);
        }
    }

    IEnumerator setHueArrayAsync(float oldHue, float newHue, Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            yield return StartCoroutine(setHueSpriteAsync(oldHue, newHue, sprites, i));
        }
    }

    void setHueSprite(float oldHue, float newHue, Sprite sprite)
    {
        Rect bounds = sprite.rect;
        Texture2D tex = sprite.texture;

        for (int y = Mathf.RoundToInt(bounds.yMin); y < Mathf.RoundToInt(bounds.yMax); y++)
        {
            int xMin = Mathf.RoundToInt(bounds.xMin), xMax = Mathf.RoundToInt(bounds.xMax);
            for (int x = xMin; x < xMax; x++)
            {
                Color temp = tex.GetPixel(x, y);
                float hue, saturation, value;
                HSVColor.RGBToHSV(temp, out hue, out saturation, out value);
                temp = HSVColor.HSVToRGB((hue + (newHue - oldHue) + 1) % 1, saturation, value, temp.a);
                tex.SetPixel(x, y, temp);
            }
        }
        tex.Apply();
    }

    IEnumerator setHueSpriteAsync(float oldHue, float newHue, Sprite[] sprites, int index)
    {
        setHueSprite(oldHue, newHue, sprites[index]);
        yield return null;
    }

    public override Sprite selectionSprite(Vector2 visualSpaceInput)
    {
        if (selectSprite == null)
        {
            selectSprite = duplicateSprite(selectionSprite());
            selectionShift = visualsToHueVector(Vector2.zero);
            setHueSprite(0, selectionShift.x, selectSprite);
        }

        setHueSprite(visualsToHueVector(selectionShift).x, visualsToHueVector(visualSpaceInput).x, selectSprite);
        selectionShift = visualSpaceInput;
        return selectSprite;
    }

    public override Sprite selectionSprite()
    {
        return base.selectionSprite(Vector2.zero); //should return one of the sprites we actually use
    }

    void OnDestroy()
    {
        DestroySpriteArray(upSprites);
        DestroySpriteArray(downSprites);
        DestroySpriteArray(leftSprites);
        DestroySpriteArray(rightSprites);
        DestroySprite(selectSprite);
    }

    void DestroySpriteArray(Sprite[] sprites)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            DestroySprite(sprites[i]);
        }
    }

    void DestroySprite(Sprite sprite)
    {
        Destroy(sprite);
    }
}

public interface IHueShiftableVisuals
{
    Vector2 shift { get; set; }
    Vector2 shiftAsync { set; }
    Vector2 visualsToHueVector(Vector2 characterVisualsVector);
}