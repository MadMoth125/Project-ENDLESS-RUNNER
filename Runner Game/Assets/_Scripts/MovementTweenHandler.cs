using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Obsolete("This class is no longer used. All tweening is now handled in the PlayerController class.")]
public class MovementTweenHandler : MonoBehaviour
{
	private TweenCallback _onComplete;
	
	[Header("Components")]
	public GameObject standingVisual;
	public GameObject crouchVisual;
	
	[Space]
	[Header("Crouch Tweening")]
	public float crouchTweenInSpeed = 1f;
	public float crouchTweenOutSpeed = 1f;
	[Space]
	[Header("Jump Tweening")]
	public float jumpTweenInSpeed = 1f;
	public float jumpTweenOutSpeed = 1f;
	public float jumpHeight = 1f;
	
	private PlayerController _playerController;

	private Sequence _crouchSequenceIn;
	private Sequence _crouchSequenceOut;
	
	private Sequence _jumpSequenceIn;
	private Sequence _jumpSequenceOut;
	
	private bool _crouchPlayingForward;
	
	private void Awake()
	{
		_playerController = GetComponent<PlayerController>();
		
		InitCrouchVisuals(); // initialize the crouch visuals

		InitTweenCompleteEvent(); // initialize the tween complete event

		_crouchSequenceOut.Pause(); // pause the sequence

		void InitCrouchVisuals()
		{
			crouchVisual.transform.localScale = new Vector3(1.2f, 1.5f, 1.2f); // pre-scale the sphere
			crouchVisual.SetActive(false); // disable the crouch visual
		}

		void InitTweenCompleteEvent()
		{
			_onComplete = () => // create a callback that will be called when the sequence is complete
			{
				if (_crouchPlayingForward)
				{
					crouchVisual.SetActive(true);
					standingVisual.SetActive(false);
				}
				else
				{
					crouchVisual.SetActive(false);
					standingVisual.SetActive(true);
				}
			};
		}
	}

	private void OnEnable()
	{
		_playerController.OnCrouchBegin += OnCrouchBegin;
		_playerController.OnCrouchEnd += OnCrouchEnd;
		_playerController.OnJumpBegin += OnJumpBegin;
	}

	private void OnDisable()
	{
		_playerController.OnCrouchBegin -= OnCrouchBegin;
		_playerController.OnCrouchEnd -= OnCrouchEnd;
		_playerController.OnJumpBegin -= OnJumpBegin;
	}
	
	private void OnCrouchBegin()
	{
		PlayCrouchTweenIn(crouchTweenInSpeed);
		
		_crouchSequenceIn.Play();
	}

	private void OnCrouchEnd()
	{
		PlayCrouchTweenOut(crouchTweenOutSpeed);
		
		_crouchSequenceOut.Play();
	}

	private void OnJumpBegin()
	{
		PlayJumpTweenIn(jumpTweenInSpeed);
		
		_crouchSequenceIn.Play();
	}
	
	private void OnJumpEnd()
	{
		PlayJumpTweenOut(jumpTweenOutSpeed);
		
		_crouchSequenceOut.Play();
	}
	
	private void PlayCrouchTweenIn(float speed)
	{
		_playerController.DisableCrouchInput();
		
		_crouchPlayingForward = true;
		
		_crouchSequenceIn = DOTween.Sequence().Pause();
		
		_crouchSequenceIn.Append(standingVisual.transform.DOScale( // shrink capsule
				new Vector3(0.9f, 0.4f, 0.9f),
				0.25f * speed)
				.OnComplete(_onComplete)
				.SetEase(Ease.InBack));
		_crouchSequenceIn.Append(crouchVisual.transform.DOScale( // shrink sphere
				new Vector3(1f, 1f, 1f),
				0.2f * speed)
				.SetEase(Ease.OutBounce)
				.OnComplete(() => _playerController.EnableCrouchInput())
		);
	}

	private void PlayCrouchTweenOut(float speed)
	{
		_playerController.DisableCrouchInput();
		
		_crouchPlayingForward = false;
		
		_crouchSequenceOut = DOTween.Sequence().Pause();
		
		_crouchSequenceOut.Append(crouchVisual.transform.DOScale( // shrink sphere
				new Vector3(1.2f, 0.8f, 1.2f),
				0.15f * speed)
				.SetEase(Ease.InExpo));
		_crouchSequenceOut.Append(crouchVisual.transform.DOScale( // expand sphere
				new Vector3(0.95f, 1.5f, 0.95f),
				0.25f * speed)
				.SetEase(Ease.OutExpo)
				.OnComplete(_onComplete));
		_crouchSequenceOut.Append(standingVisual.transform.DOScale( // shrink capsule
				new Vector3(1f, 1f, 1f),
				0.25f * speed)
				.SetEase(Ease.OutBounce)
				.OnComplete(() => _playerController.EnableCrouchInput())
		);
	}

	private float _lastY;
	
	private void PlayJumpTweenIn(float speed)
	{
		_lastY = transform.position.y;
		
		_playerController.DisableJumpInput();
		
		transform.DOMoveY(_lastY + jumpHeight, jumpTweenInSpeed)
			.SetEase(Ease.OutExpo)
			.OnComplete(() => PlayJumpTweenOut(speed)
			);
	}
	
	private void PlayJumpTweenOut(float speed)
	{
		transform.DOMoveY(_lastY, jumpTweenOutSpeed)
			.SetEase(Ease.InExpo)
			.OnComplete(() => _playerController.EnableJumpInput()
			);
	}
}