using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainSegmentChoices", menuName = "ScriptableObjects/Terrain Segment Collection", order = 1)]
public class TerrainSelection_SO : ScriptableObject
{
	[SerializeField]
	private List<TerrainSegment> terrainSegments = new List<TerrainSegment>();
	
	public ITerrainSegment GetNextTerrain()
	{
		if (terrainSegments.Count == 0)
		{
			Debug.LogError("There are no terrain segments in the collection!");
			return null;
		}
		
		return terrainSegments[UnityEngine.Random.Range(0, terrainSegments.Count)];
	}
}