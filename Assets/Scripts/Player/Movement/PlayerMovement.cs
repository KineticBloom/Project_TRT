using NaughtyAttributes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	#region ======== [ OBJECT REFERENCES ] ========

	[Header("Object References")]
	[SerializeField] private Transform forwardTransform;
	[SerializeField] private Animator animator;
	[SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite spriteIdle;
    [SerializeField] private Sprite spriteWalk;
	[SerializeField] private PlayerSFX playerSFX;
	[SerializeField] private ParticleSystem dustTrail;
    private CharacterController _characterController;

	#endregion

	#region ======== [ PARAMETERS ] ========

	[Header("Parameters")]
	[SerializeField] private float speed = 5f;

	#endregion

	#region ======== [ PRIVATE PROPERTIES ] ========

	[SerializeField, ReadOnly] private float _adjustedSpeed = 5f;
	private const float _gravity = 9.81f;
	private float _downwardForce = 0;
	private bool _isWalking = false;
	public bool IsWalking => _isWalking;
	private bool _wasWalking = false;
    private Vector3 _input = Vector3.zero;
	public Vector3 Input => _input;
	[SerializeField, ReadOnly] private bool _canMove = true;
	private bool _forcedToMove = false;
	private Vector3 _forcedInput = Vector3.zero;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Public setter for _canMove.
    /// </summary>
    /// <param name="canMove">bool - whether or not the player can move.</param>
	public void SetCanMove(bool canMove) {
		_canMove = canMove;
	}
	
	/// <summary>
	/// Set speed to mult times default speed
	/// </summary>
	/// <param name="mult">The value to multiply by</param>
	public void SetSpeed(float mult)
	{
		_adjustedSpeed = speed * mult;
	}
	
	/// <summary>
	/// Resets speed to default
	/// </summary>
	public void ResetSpeed()
	{
		_adjustedSpeed = speed;
	}
	
	/// <summary>
	/// Force the player character to move
	/// </summary>
	public void ForceMove(bool isMoving, Vector3 dir)
	{
		_forcedToMove = isMoving;
		_forcedInput = dir;
	}

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    void Start()
	{
		_characterController = GetComponent<CharacterController>();
		_adjustedSpeed = speed;
		dustTrail.Stop();
	}

	void Update()
	{
		UpdateMovement();
		UpdateGravity();
	}

	private void UpdateMovement()
	{
		// Get Input
		_input = GameManager.PlayerInput.GetControlInput();

		if (!_canMove) {
			_input = Vector3.zero;
		}
		if (_forcedToMove)
		{
			_input = _forcedInput;
		}

		// Relative to Target
		float y = forwardTransform.rotation.eulerAngles.y;
		Quaternion targetRotation = Quaternion.Euler(new Vector3(0, y, 0));

		// Move character
		Vector3 direction = targetRotation * _input;
		_characterController.Move(_adjustedSpeed * Time.deltaTime * direction);

		//animator.speed = Mathf.Min(1,(direction * speed).magnitude);

		if (spriteRenderer)
        {
			if (_input == Vector3.zero)
			{
				spriteRenderer.sprite = spriteIdle;
			}
			else
			{
				spriteRenderer.sprite = spriteWalk;

				if (direction.x > 0)
				{
					spriteRenderer.flipX = false;
				}
				else
				{
					spriteRenderer.flipX = true;
				}
			}
		}

		_isWalking = (direction * _adjustedSpeed).magnitude > 0;

		HandleDustTrail();

        if (animator)
        {
			animator.SetBool("IsWalking", _isWalking);
		}
	}

	private void UpdateGravity()
	{
		if (!_characterController.isGrounded) {
			_downwardForce += _gravity * Time.deltaTime;
			_characterController.Move(_downwardForce * Time.deltaTime * Vector3.down);
		} else {
			_downwardForce = 0;
		}
	}

	private void HandleDustTrail()
	{
        if (_isWalking != _wasWalking)
        {
            if (_isWalking)
            {
                // Started walking
                dustTrail.Play();
            }
            else
            {
                // Stopped walking
                dustTrail.Stop();
            }
        }
        _wasWalking = _isWalking;
    }
	#endregion
}
