using UnityEngine;
using System.Collections;

public static class SideExtension
{
    public static bool onSide(this Side side, Vector2 position)
    {
        switch (side)
        {
            case Side.LEFT:
                return position.x <= 0;
            case Side.RIGHT:
                return position.x >= 0;
        }
        return false;
    }
}