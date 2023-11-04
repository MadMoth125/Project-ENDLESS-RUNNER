using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;
using Sequence = DG.Tweening.Sequence;

public class PlayerController : MonoBehaviour
{
	public event Action OnCrouchBegin, OnCrouchEnd;
	public event Action OnJumpBegin, OnJumpEnd;
	
	[Header("Components")]
	public GameObject standingVisual;
	public GameObject crouchVisual;
	private BoxCollider _playerCollider;
	
	[Space(10)]
	[Header("Movement Settings")]
	[SerializeField]
	[Tooltip("The positions that the player can move to.\n" +
	         "Should be set in order from left to right.")]
	private Transform[] movementPositions = new Transform[3];
	[SerializeField]
	private float jumpHeight = 1f;
	
	[Space(10)]
	[Header("Movement Animation Settings")]
	[Tooltip("The duration of the movement animation when moving between positions.")]
	public float movementTweenSpeed = 0.3f;
	private int _currentPositionIndex = 1; // the index of the current position in the movementPositions array
	
	[Space]
	public float crouchTweenInDuration = 1f;
	public float crouchTweenOutDuration = 1f;
	private Sequence _crouchSequenceIn;
	private Sequence _crouchSequenceOut;
	private bool _crouchPlayingForward;
	
	[Space]
	public float jumpTweenInDuration = 1f;
	public float jumpTweenOutDuration = 1f;
	private Sequence _jumpSequence;
	
	private bool _isMovingHorizontal; // whether or not the player can move horizontally (is in the middle of a movement)
	private bool _isCrouching; // whether or not the player is crouching
	private bool _isJumping; // whether or not the player is jumping
	private float _previousLocationY; // the y position of the player before the jump

	private bool _canMove = true;
	private bool _canCrouch = true;
	private bool _canJump = true;
	
	private DoOnce _onLandedDoOnce = new DoOnce(true);
	private TweenCallback _onCrouchTransition;
	
	private void Awake()
	{
		_playerCollider = GetComponent<BoxCollider>();
		
		_isMovingHorizontal = false;
		_isCrouching = false;
		_isJumping = false;
		
		_onCrouchTransition = () => // create a callback that will be called when the sequence is complete
		{
			if (_crouchPlayingForward)
			{
				// swap the active visuals
				crouchVisual.SetActive(true);
				standingVisual.SetActive(false);
				crouchVisual.transform.localScale = new Vector3(0.95f, 1.5f, 0.95f);
				
				// change the collider size and position
				_playerCollider.center = new Vector3(0f, 0.5f, 0f);
				_playerCollider.size = new Vector3(1f, 1f, 1f);
			}
			else
			{
				crouchVisual.SetActive(false);
				standingVisual.SetActive(true);
				
				_playerCollider.center = new Vector3(0f, 1f, 0f);
				_playerCollider.size = new Vector3(1f, 2f, 1f);
			}
		};
	}

	private void Update()
	{
		bool leftInput =  _canMove && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow));
		bool rightInput = _canMove && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow));
		bool crouchInput = _canCrouch && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow));
		bool jumpInput = _canJump && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow));
		
		DetectOnGround();
		
		if (leftInput ^ rightInput && !_isMovingHorizontal)
		{
			_isMovingHorizontal = true;
			
			if (leftInput) MoveToPosition(-1);
			if (rightInput) MoveToPosition(1);
		}

		if (crouchInput)
		{
			CheckCrouch();
		}

		if (jumpInput)
		{
			CheckJump();
		}
	}
	
	private void MoveToPosition(int indexDirection)
	{
		if (_currentPositionIndex + indexDirection < 0 || _currentPositionIndex + indexDirection >= movementPositions.Length)
		{
			Debug.LogWarning($"Invalid position index {_currentPositionIndex + indexDirection} passed to {GetType()}");
			_isMovingHorizontal = false;
			return;
		}
		
		_currentPositionIndex += indexDirection;
		
		transform.DOMoveX(
			movementPositions[_currentPositionIndex].position.x,
			movementTweenSpeed)
			.OnComplete(() => _isMovingHorizontal = false);
	}

	private void CheckCrouch()
	{
		if (!_isCrouching)
		{
			PlayCrouchTweenIn();
			OnCrouchBegin?.Invoke();
		}
		else
		{
			PlayCrouchTweenOut();
			OnCrouchEnd?.Invoke();
		}
		
		_isCrouching = !_isCrouching;
	}
	
	private void CheckJump()
	{
		if (!_isJumping)
		{
			Debug.Log("Jump");
			
			_isJumping = true;
			
			_onLandedDoOnce.Reset();
			
			PlayJumpTween();
			OnJumpBegin?.Invoke();
		}
	}

	private void DetectOnGround()
	{
		if (_isJumping && Physics.Raycast(transform.position, Vector3.down, out _, 0.5f))
		{
			_onLandedDoOnce.Do(() =>
			{
				_isJumping = false;
				OnJumpEnd?.Invoke();
			});
		}
	}
	
	#region Enable / Disable Input

	public void EnableMovementInput() => _canMove = true;
	public void DisableMovementInput() => _canMove = false;
	
	public void EnableCrouchInput() => _canCrouch = true;
	public void DisableCrouchInput() => _canCrouch = false;
	
	public void EnableJumpInput() => _canJump = true;
	public void DisableJumpInput() => _canJump = false;

	#endregion

	#region Tweening Functions

	private void PlayCrouchTweenIn()
	{
		DisableCrouchInput();
		
		_crouchPlayingForward = true;
		
		_crouchSequenceIn = DOTween.Sequence().Pause();
		
		_crouchSequenceIn.Append(standingVisual.transform.DOScale( // shrink capsule
				new Vector3(0.9f, 0.4f, 0.9f),
				0.25f * crouchTweenInDuration)
			.OnComplete(_onCrouchTransition)
			.SetEase(Ease.InBack));
		
		_crouchSequenceIn.Append(crouchVisual.transform.DOScale( // shrink sphere
				new Vector3(1f, 1f, 1f),
				0.2f * crouchTweenInDuration)
			.SetEase(Ease.OutBounce)
			.OnComplete(EnableCrouchInput)
		);
		
		_crouchSequenceIn.Play();
	}

	private void PlayCrouchTweenOut()
	{
		DisableCrouchInput();
		
		_crouchPlayingForward = false;
		
		_crouchSequenceOut = DOTween.Sequence().Pause();
		
		_crouchSequenceOut.Append(crouchVisual.transform.DOScale( // shrink sphere
				new Vector3(1.2f, 0.8f, 1.2f),
				0.15f * crouchTweenOutDuration)
			.SetEase(Ease.InExpo));
		
		_crouchSequenceOut.Append(crouchVisual.transform.DOScale( // expand sphere
				new Vector3(0.95f, 1.5f, 0.95f),
				0.25f * crouchTweenOutDuration)
			.SetEase(Ease.OutExpo)
			.OnComplete(_onCrouchTransition));
		
		_crouchSequenceOut.Append(standingVisual.transform.DOScale( // shrink capsule
				new Vector3(1f, 1f, 1f),
				0.25f * crouchTweenOutDuration)
			.SetEase(Ease.OutBounce)
			.OnComplete(EnableCrouchInput)
		);
		
		_crouchSequenceOut.Play();
	}
	
	private void PlayJumpTween()
	{
		DisableJumpInput();
		
		_previousLocationY = transform.position.y;
		
		_jumpSequence = DOTween.Sequence().Pause();
		
		if (_isCrouching)
		{
			CheckCrouch();
		}
		
		_jumpSequence.Append(transform.DOMoveY( // jump up
				_previousLocationY + jumpHeight,
				jumpTweenInDuration)
				.SetEase(Ease.OutExpo));
		
		_jumpSequence.Append(transform.DOMoveY( // fall down
				_previousLocationY,
				jumpTweenOutDuration)
				.SetEase(Ease.OutBounce) // meant to look like the player is bouncing upon landing
				.OnComplete(EnableJumpInput)
		);
		
		_jumpSequence.Play();
	}
	
	// currently, we only need to stop the jump tween for preventing any weird behavior.
	// crouch tweens arenot stopped because they don't cause any weird behavior.
	public void StopAllTweens()
	{
		// _crouchSequenceIn?.Kill();
		// _crouchSequenceOut?.Kill();
		_jumpSequence?.Kill();
	}
	
	#endregion
	
	public enum PlayerState
	{
		Standing,
		Crouching,
		Jumping
	}
}