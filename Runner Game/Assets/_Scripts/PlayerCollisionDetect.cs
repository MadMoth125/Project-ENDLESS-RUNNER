using System;
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
			// call our DoOnce when we hit a hazard
			_onPlayerHitHazardDoOnce.Do(PlayerHitHazard);
		}

		void PlayerHitHazard()
		{
			// a direction going away from the hazard
			Vector3 hitDirection = -(other.transform.position - transform.position).normalized;
			
			// random force values to add to the player's rigidbody
			float randomUpwardForce = UnityEngine.Random.Range(0.5f, 3f);
			float randomForwardForce = UnityEngine.Random.Range(0.25f, 2f);
			float randomTorqueForce = UnityEngine.Random.Range(-3, 4);
			
			// pause/stop any active tweens (defined by the playerController)
			_playerController.StopAllTweens();
			
			// disable the player controller
			_playerController.enabled = false;
			
			// disable the player's rigidbody constraints
			_rigidbody.constraints = RigidbodyConstraints.None;

			// add forces to the player's rigidbody
			_rigidbody.AddForce(
				(Vector3.up * randomUpwardForce +
				 Vector3.forward * randomForwardForce) +
				 hitDirection,
				 ForceMode.Impulse);
			_rigidbody.AddTorque(
				Vector3.right * randomTorqueForce,
				ForceMode.Impulse);

			// invoke the event
			OnPlayerHitHazard?.Invoke();
		}
	}
}