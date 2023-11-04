using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DistanceFromTarget : MonoBehaviour
{
	private ObjectPanner _objectPanner;
	private TextMeshPro _textMeshPro;

	private void Awake()
	{
		_objectPanner = GetComponent<ObjectPanner>();
		_textMeshPro = GetComponentInChildren<TextMeshPro>();
	}

	private void Update()
	{
		// _textMeshPro.text = $"Distance: {Mathf.RoundToInt(Vector3.Distance(transform.position, _objectPanner.locationTarget))}";
	}
}