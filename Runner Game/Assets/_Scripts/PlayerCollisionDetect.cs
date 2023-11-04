using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utilities;

public class PlayerCollisionDetect : MonoBehaviour
{
	public event Action OnPlayerHitHazard;
	
	private PlayerController _playerController;
	private BoxCollider _playerCollider;
	private Rigidbody _rigidbody;
	
	private DoOnce _onPlayerHitHazardDoOnce = new DoOnce();
	
	private void Awake()
	{
		_playerController = GetComponent<PlayerController>();
		_playerCollider = GetComponent<BoxCollider>();
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Hazard>())
		{
			_onPlayerHitHazardDoOnce.Do(PlayerHitHazard);
		}

		void PlayerHitHazard()
		{
			Vector3 hitDirection = -(other.transform.position - transform.position).normalized;
			float randomUpwardForce = UnityEngine.Random.Range(0.5f, 3f);
			float randomForwardForce = UnityEngine.Random.Range(0.25f, 2f);
			
			// pause any active tweens (defined by the playerController)
			_playerController.StopAllTweens();
			
			// disable the player controller
			_playerController.enabled = false;
			
			// disable the player's rigidbody constraints
			_rigidbody.constraints = RigidbodyConstraints.None;

			// add forces to the player's rigidbody
			_rigidbody.AddForce(
				(Vector3.up * randomUpwardForce +
				 Vector3.forward * randomForwardForce) +
				 hitDirection * 2f,
				 ForceMode.Impulse);
			_rigidbody.AddTorque(
				Vector3.right * randomForwardForce,
				ForceMode.Impulse);

			// invoke the event that the player has hit a hazard
			OnPlayerHitHazard?.Invoke();
		}
	}
}