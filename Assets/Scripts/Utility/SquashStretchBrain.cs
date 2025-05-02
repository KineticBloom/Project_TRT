using UnityEngine;

// ================================================================
//
// IMPORTANT NOTE:
// This script scales transforms from their PIVOT.
//
// If an objects's pivot is at its BOTTOM, like for the player and
// NPC prefabs, it will appear to squash and stretch from the 
// bottom of the object.
//
// If an object's pivot is at its CENTER, like by default, it will
// appear to squash and stretch from its center. 
// THIS MAY LOOK WRONG.
//
// ================================================================

public class SquashStretchBrain : MonoBehaviour
{
    // Parameters =================================================================================

    [Header("Squash-Stretch Parameters")]
    [Tooltip("A float (-1 to 1) which is used to lerp the base scale towards squash, if SquashStretch is "
           + "negative, or towards stretch, if SquashStretch is positive.")]
    [Range(-1.0f, 1.0f)]
    public float SquashStretch = 0;
    [Tooltip("The magnitude of squash / stretch. When magnitude is 2, max stretch is 2x as tall "
           + "and 0.5x as wide as the rest state. When magnitude is 5, max stretch is 5x as tall "
           + "and 0.2x as wide as the rest state. Etc.\n\nDefault: 2")]
    [SerializeField]
    private float magnitude = 1.05f;

    // Misc internal variables ====================================================================

    // Updated whenever SquashStretch changes to store the previous value of SquashStretch. We 
    // compare this to the current value of SquashStretch to detect if it has changed.
    private float _lastSquashStretch;
    // The transform's base scale, obtained on init.
    private Vector3 _baseLocalScale;

    // Initializers ===============================================================================

    private void Start()
    {
        // Start is called before the first frame update. Used to initialize all our variables 
        // and define all our references.
        // ================

        SquashStretch = _lastSquashStretch = 0;
        _baseLocalScale = transform.localScale;
    }

    // Update methods =============================================================================

    private void Update()
    {
        // Update is called once per frame. We use it to detect when SquashStretch changes and to
        // call the update function on it when it does.
        // ================

        // If SquashStretch has changed, update SquashStretch.
        if ( SquashStretch != _lastSquashStretch ) {
            UpdateSquashStretch();
            _lastSquashStretch = SquashStretch;
        }
    }

    private void UpdateSquashStretch()
    {
        // Called when SquashStretch changes. If SquashStretch is 0, resets scale to baseScale. If it
        // is above 0, lerps towards full stretch. If it is below 0, lerps towards full squash.
        // ================

        float width = Mathf.Pow(magnitude, -SquashStretch);
        float height = Mathf.Pow(magnitude, SquashStretch);
        transform.localScale = Vector3.Scale(new(width, height, 1f), _baseLocalScale);
    }
}