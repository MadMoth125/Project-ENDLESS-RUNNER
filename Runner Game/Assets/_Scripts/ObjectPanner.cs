using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPanner : MonoBehaviour
{
	[HideInInspector]
	public Vector3 locationTarget;
	[HideInInspector]
	public float lastKnownSpeed;
	
	public void SetTargetLocation(Vector3 targetLocation)
	{
		locationTarget = targetLocation;
	}

	public void TranslateInDirection(float speed, Vector3 direction)
	{
		lastKnownSpeed = Time.deltaTime * speed;
		transform.position += direction * lastKnownSpeed;
	}
	
	public bool IsWithinTargetThreshold(float threshold)
	{
		Debug.Log(threshold * lastKnownSpeed);
		return Vector3.Distance(transform.position, locationTarget) < Mathf.Max(threshold, threshold * lastKnownSpeed);
	}
	
	public bool IsWithinTargetThreshold(float threshold, Axis axis)
	{
		switch (axis)
		{
			case Axis.X:
				return Mathf.Abs(transform.position.x - locationTarget.x) < threshold * lastKnownSpeed;
			case Axis.Y:
				return Mathf.Abs(transform.position.y - locationTarget.y) < threshold * lastKnownSpeed;
			default:
			case Axis.Z:
				return Mathf.Abs(transform.position.z - locationTarget.z) < threshold * lastKnownSpeed;
		}
	}
	
	public enum Axis
	{
		X,
		Y,
		Z
	}
}