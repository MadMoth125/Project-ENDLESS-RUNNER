using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
	public int targetFrameRate = 144;
	
	private void Start()
	{
		Application.targetFrameRate = targetFrameRate;
	}
}