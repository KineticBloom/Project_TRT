using UnityEngine;

public class SquashStretch : MonoBehaviour
{
    // Parameters =================================================================================

    [Header("Squash-Stretch Parameters")]
    [Tooltip("A float (-1 to 1) which is used to lerp the base scale towards squash, if squashStretch is "
           + "negative, or towards stretch, if squashStretch is positive.")]
    [Range(-1.0f, 1.0f)]
    public float squashStretch = 0;
    [Tooltip("The magnitude of squash / stretch. When magnitude is 2, max stretch is 2x as tall "
           + "and 0.5x as wide as the rest state. When magnitude is 5, max stretch is 5x as tall "
           + "and 0.2x as wide as the rest state. Etc.\n\nDefault: 2")]
    [SerializeField]
    private float magnitude = 2;

    // Internal variables =========================================================================

    // Updated whenever squashStretch changes to store the previous value of squashStretch. We compare
    // this to the current value of squashStretch to detect if it has changed.
    private float _lastSquashStretch;
    // The transform's base scale, obtained on init.
    private Vector3 _baseLocalScale;

    // Initializers ===============================================================================

    private void Start()
    {
        // Start is called before the first frame update. Used to initialize all our variables 
        // and define all our references.
        // ================

        squashStretch = _lastSquashStretch = 0;
        
        _baseLocalScale = transform.localScale;
    }

    // Update methods =============================================================================

    private void Update()
    {
        // Update is called once per frame. We use it to detect when squashStretch changes and to
        // call the update function on it when it does.
        // ================

        // If squashStretch has changed, update squashStretch.
        if ( squashStretch != _lastSquashStretch ) {
            UpdateSquashStretch();
            _lastSquashStretch = squashStretch;
        }
    }

    private void UpdateSquashStretch()
    {
        // Called when squashStretch changes. If squashStretch is 0, resets scale to baseScale. If it
        // is above 0, lerps towards full stretch. If it is below 0, lerps towards full squash.
        // ================

        float width = Mathf.Pow(magnitude, -squashStretch);
        float height = Mathf.Pow(magnitude, squashStretch);
        transform.localScale = Vector3.Scale(new(width, height, 1f), _baseLocalScale);
    }
}