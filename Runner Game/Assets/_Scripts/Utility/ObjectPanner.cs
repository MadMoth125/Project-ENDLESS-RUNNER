using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPanner : MonoBehaviour
{
	public Vector3 LocationTarget { get; private set; }
	
	public void TranslateInDirection(float speed, Vector3 direction)
	{
		transform.position += direction * speed;
	}
	
	public bool IsWithinTargetThreshold(Vector3 objectPosition, Vector3 targetPosition, float threshold)
    {
	    LocationTarget = targetPosition;
	    
        return Vector3.Distance(objectPosition, targetPosition) < threshold;
    }
	
	/*private bool IsWithinTargetThreshold(float threshold, Axis axis)
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
	}*/
	
	public enum Axis
	{
		X,
		Y,
		Z
	}
}