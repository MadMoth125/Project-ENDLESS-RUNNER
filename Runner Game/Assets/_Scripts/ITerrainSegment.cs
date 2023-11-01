using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerrainSegment
{
	public Transform TerrainTransform { get; }
	public Transform StartTransform { get; }
	public Transform EndTransform { get; }
	
	/// <summary>
	/// Meant to be called when the terrain needs to be translated in a direction
	/// </summary>
	/// <param name="deltaDistance"></param>
	/// <param name="translationDirection"></param>
	public void TranslateTerrain(float deltaDistance, Vector3 translationDirection);

	public bool IsWithinTargetThreshold(Vector3 target, float threshold);
	
	/// <summary>
	/// Gets the difference between the terrain's <see cref="Transform"/> position and the <see cref="StartTransform"/> position
	/// </summary>
	/// <returns>A <see cref="Vector3"/> of the difference in position</returns>
	public Vector3 GetTerrainOffsetPosition();
	
	/// <summary>
	/// Gets the next terrain segment based on the <see cref="TerrainSegment"/>'s <see cref="TerrainSelection_SO"/>
	/// </summary>
	/// <returns>The terrain segment determined by the <see cref="TerrainSegment"/></returns>
	public ITerrainSegment GetNextTerrainSegment();
}