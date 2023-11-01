using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Movement Settings")]
	[SerializeField]
	private float horizontalRange = 5f;

	private void Update()
	{
		Vector3 newPosition = transform.position + Vector3.right * (Time.deltaTime * Input.GetAxisRaw("Horizontal") * 5f);
		
		newPosition.x = Mathf.Clamp(newPosition.x, -horizontalRange, horizontalRange);
		
		transform.position = newPosition;
	}
}