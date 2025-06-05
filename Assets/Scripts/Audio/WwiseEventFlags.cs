using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WwiseEventFlags
{
    public static bool canPlayHighlightSFX = false;

    public static void SetCanPlayHighlightSFX(bool newValue)
    {
        canPlayHighlightSFX = newValue;
    }
}
