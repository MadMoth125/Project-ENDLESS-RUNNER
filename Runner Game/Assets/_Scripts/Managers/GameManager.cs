using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public event Action OnPlayerDie;
	public static GameManager Instance { get; private set; }
	
	private PlayerCollisionDetect _playerCollisionDetect;
	
	public bool IsGameActive { get; private set; } = true;
	
	#region Unity Methods

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Debug.LogError($"There is already an instance of {GetType()} in the scene!");
			Destroy(gameObject);
			return;
		}
	}

	private void OnEnable()
	{
		SubToEvents();
	}
	
	private void OnDisable()
	{
		UnsubFromEvents();
	}
	
	private void Start()
	{
		_playerCollisionDetect = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerCollisionDetect>();

		SubToEvents();
	}

	#endregion

	#region Sub / Unsub Events

	private void SubToEvents()
	{
		if (_playerCollisionDetect == null)
		{
			/*Debug.LogWarning($"SUBSCRIBE EVENT: " +
			                 $"There is no valid \"{_playerCollisionDetect.GetType()}\" reference found! " +
			                 $"Make sure the player has a \"{_playerCollisionDetect.GetType()}\" component on it" +
			                 "Or make sure there is a valid GameObject with the tag of \"Player\" in the scene!");*/
			Debug.LogWarning($"{GetType()}: Failed to subscribe to event!");
		}
		else
		{
			Debug.Log($"{GetType()}: subscribed to event!");
			_playerCollisionDetect.OnPlayerHitHazard += PlayerHitHazard;
		}
	}

	private void UnsubFromEvents()
	{
		if (_playerCollisionDetect == null)
		{
			/*Debug.LogWarning($"UNSUBSCRIBE EVENT: " +
			                 $"There is no valid \"{_playerCollisionDetect.GetType()}\" reference found! " +
			                 $"Make sure the player has a \"{_playerCollisionDetect.GetType()}\" component on it. " +
			                 "Or make sure there is a valid GameObject with the tag of \"Player\" in the scene!");*/
			Debug.LogWarning($"{GetType()}: Failed to unsubscribe to event!");
		}
		else
		{
			Debug.Log($"{GetType()}: unsubscribed from event!");
			_playerCollisionDetect.OnPlayerHitHazard -= PlayerHitHazard;
		}
	}

	#endregion
	
	private void PlayerHitHazard()
	{
		IsGameActive = false;
		OnPlayerDie?.Invoke();
	}
	
	#region Game Events (deprecated)

	public delegate void GameEvent(Vector3 playerPosition, int score);
	
	private List<GameEvent> _gameEvents = new List<GameEvent>();
	
	public void RegisterGameEvent(GameEvent gameEvent)
	{
		_gameEvents.Add(gameEvent);
	}
	
	public void UnregisterGameEvent(GameEvent gameEvent)
	{
		_gameEvents.Remove(gameEvent);
	}

	private void FireGameEvent()
	{
		foreach (var gameEvent in _gameEvents)
		{
			// gameEvent?.Invoke();
		}
	}

	#endregion
}