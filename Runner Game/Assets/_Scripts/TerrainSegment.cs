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

	public void TranslateTerrain(float deltaDistance, Vector3 translationDirection)
	{
		_objectPanner.TranslateInDirection(deltaDistance, translationDirection);
	}

	public bool IsWithinTargetThreshold(Vector3 pivot, Vector3 target, float threshold)
	{
		_objectPanner.SetTargetLocation(target);
		return _objectPanner.IsWithinTargetThreshold(pivot, threshold);
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