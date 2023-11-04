using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(ObjectPanner))]
public class TerrainSegment : MonoBehaviour, ITerrainSegment
{
	public TerrainSelection_SO terrainSelection; // Scriptable object that contains all "acceptable" terrain segments

	#region ITerrain Interface
	public Transform TerrainTransform => transform; // pivot point the the GameObject that contains the terrain
	
	[field:SerializeField]
	public Transform StartTransform { get; private set; } // pivot point of the terrain should be at the start
	
	[field:SerializeField]
	public Transform EndTransform { get; private set; } // pivot point of where the terrain ends and the next terrain should begin
	#endregion

	private ObjectPanner _objectPanner; // Object panner component required for making the terrain move
	
	private void Awake()
	{
		_objectPanner = GetComponent<ObjectPanner>();
	}

	private void Update()
	{
		// acts as a failsafe to prevent the terrain from going too far
		if (transform.position.z < -100f)
		{
			gameObject.SetActive(false);
		}
	}

	public void TranslateTerrain(float distance, Vector3 translationDirection)
	{
		_objectPanner.TranslateInDirection(distance, translationDirection);
	}

	public bool IsWithinTargetThreshold(Vector3 objectPosition, Vector3 targetPosition, float threshold)
	{
		return Vector3.Distance(objectPosition, targetPosition) < threshold;
	}

	public Vector3 GetTerrainOffsetPosition()
	{
		return TerrainTransform.position - StartTransform.position;
	}

	public ITerrainSegment GetNextTerrainSegment()
	{
		return terrainSelection.GetNextTerrain();
	}
}