using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class TerrainSpawnManager : MonoBehaviour
{
	[Header("Terrain Components")]
	public List<TerrainSegment> terrainPrefabs = new List<TerrainSegment>();
	private List<ITerrainSegment> _instancedTerrain = new List<ITerrainSegment>();
	
	[Space(10)]
	[Header("Spawn Settings")]
	[SerializeField]
	[Tooltip("The transform from which the starting terrain will be spawned.\n" +
	         "\nKeep in mind that only the position will be used to spawn the terrain, so the rotation and scale of this transform are irrelevant.")]
	private Transform spawnTransformReference;
	private Vector3 _locationTarget;
	
	[Header("Spawn Settings")]
	[SerializeField]
	[Tooltip("The number of terrain prefabs to spawn at the start of the game.")]
	private int spawnBuffer = 3;
	[SerializeField]
	private float terrainMovementSpeed = 16f;
	[SerializeField]
	private float terrainTargetMeetThreshold = 1f;
	
	private TerrainSegment _nextSegment;
	private bool _initialSpawningComplete = false;
	
	private void Start()
	{
		if (terrainPrefabs.Count < 1)
		{
			Debug.LogError($"No terrain prefabs assigned to {GetType()}");
			return;
		}
		
		// begin the coroutine that will spawn the terrain
		StartCoroutine(InitializeStartingSegments());
	}

	private void Update()
	{
		if (_initialSpawningComplete)
		{
			List<ITerrainSegment> removedItems = new List<ITerrainSegment>();
			
			foreach (var segment in _instancedTerrain)
			{
				if (segment.IsWithinTargetThreshold(
					    segment.EndTransform.position,
					    _locationTarget - segment.GetTerrainOffsetPosition(),
					    terrainTargetMeetThreshold))
				{
					removedItems.Add(segment);
				}
				else
				{
					segment.TranslateTerrain(terrainMovementSpeed, Vector3.back);
				}
			}
			
			foreach (var removedItem in removedItems)
			{
				_instancedTerrain.Remove(removedItem);
				removedItem.TerrainTransform.gameObject.SetActive(false);
			}
			
			_instancedTerrain.TrimExcess();
			removedItems.Clear();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(_locationTarget, 1f);
	}
	
	///<summary>
	/// Initializes starting terrain segments based on specified spawn buffer.
	///</summary>
	private IEnumerator InitializeStartingSegments()
	{
	    // Loop through the spawnBuffer to create terrain instances.
	    for (int i = 0; i < spawnBuffer; i++)
	    {
	        ITerrainSegment tempInstance;
	        Vector3 spawnPosition;

	        // Check if it's the first iteration.
	        if (i == 0)
	        {
	            // Set spawn position for the first terrain segment.
	            spawnPosition = spawnTransformReference.position + terrainPrefabs[0].GetTerrainOffsetPosition();

	            _locationTarget = spawnPosition;
	            
	            // Spawn the initial terrain segment.
	            yield return tempInstance = SpawnSegment(terrainPrefabs[0], spawnPosition, Quaternion.identity);
	        }
	        else
	        {
	            // Set spawn position for subsequent terrain segments based on the previous one.
	            spawnPosition = _instancedTerrain[i - 1].EndTransform.position + _nextSegment.GetTerrainOffsetPosition();

	            // Spawn the next terrain segment.
	            yield return tempInstance = SpawnSegment(_nextSegment, spawnPosition, _instancedTerrain[i - 1].EndTransform.rotation);
	        }

	        // Add the instantiated terrain instance to the _instancedTerrain list.
	        _instancedTerrain.Add(tempInstance);

	        // Get the next terrain segment in the sequence.
	        _nextSegment = _instancedTerrain[^1].GetNextTerrainSegment() as TerrainSegment;
	    }

	    // Set the flag to indicate that the initial spawning is complete.
	    _initialSpawningComplete = true;
	}

	///<summary>
	/// Spawns a terrain segment at the specified position and rotation.
	///</summary>
	/// <param name="segment">Terrain segment to be spawned.</param>
	/// <param name="spawnPosition">Position for the new terrain segment.</param>
	/// <param name="spawnRotation">Rotation for the new terrain segment.</param>
	/// <returns>Instantiated terrain segment.</returns>
	private ITerrainSegment SpawnSegment(TerrainSegment segment, Vector3 spawnPosition, Quaternion spawnRotation)
	{
	    return Instantiate(segment, spawnPosition, spawnRotation);
	}
}