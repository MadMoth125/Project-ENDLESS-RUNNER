using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
	public static ScoreManager Instance { get; private set; }
	
	[Header("Component References")]
	public TextMeshProUGUI scoreText;
	
	[Space(10)]
	[Header("Score Settings")]
	[Tooltip("The time it takes for the player to get score from the passive score timer.")]
	public float countdownTime = 0.25f;  // Set the initial countdown time in seconds
	private float _timer;
	
	private int _score = 0;
	private int _passiveScore = 0;

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
		GameManager.Instance.OnPlayerDie += OnPlayerDie;
	}

	private void OnDisable()
	{
		GameManager.Instance.OnPlayerDie -= OnPlayerDie;
	}
	
	private void Start()
	{
		_timer = countdownTime;
		scoreText.text = $"Score: {_score + _passiveScore}";
	}

	private void Update()
	{
		TimerHandler(Time.deltaTime, TimerTick);
	}
	
	#endregion
	
	private void AddPassiveScore(int amount)
	{
		_passiveScore += amount;
		scoreText.text = $"Score: {_score + _passiveScore}";
	}

	private void OnPlayerDie()
	{
		enabled = false;
	}
	
	#region Timer

	private void TimerHandler(float deltaTime, Action action)
	{
		_timer -= deltaTime;
		if (_timer <= 0f)
		{
			_timer = countdownTime;
			TimerTick();
		}
	}
	
	private void TimerTick()
	{
		AddPassiveScore(1);
	}

	#endregion
}