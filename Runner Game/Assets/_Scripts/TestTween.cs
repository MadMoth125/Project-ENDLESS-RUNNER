using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TestTween : MonoBehaviour
{
	private Sequence scaleSequence;
	private bool isPlayingForward = true;

	void Start()
	{
		// Initialize the DOTween sequence
		scaleSequence = DOTween.Sequence();

		// Set up the scaling animations
		scaleSequence.Append(transform.DOScale(Vector3.one * 2f, 1f).SetAutoKill(false)); // Scale to twice the size in 1 second
		scaleSequence.Append(transform.DOScale(Vector3.one, 1f).SetAutoKill(false)); // Scale back to the original size in 1 second
		// scaleSequence.Append(scaleSequence.Pause());

		// Pause the sequence initially
		scaleSequence.Pause();
		scaleSequence.SetAutoKill(false);
	}

	/*void Update()
	{
		// Check if the space key is pressed
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// Toggle between playing forwards and backwards
			if (isPlayingForward)
			{
				scaleSequence.PlayForward();
			}
			else
			{
				scaleSequence.PlayBackwards();
			}

			// Update the direction flag
			isPlayingForward = !isPlayingForward;
		}
	}*/
	
	void Update()
	{
		// Check if the space key is pressed
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// Restart the sequence from the beginning
			scaleSequence.Restart();
		}
	}
}