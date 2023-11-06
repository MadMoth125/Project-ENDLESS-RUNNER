using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class TerrainSpawnManager : MonoBehaviour
{
	[Header("Terrain Components")]
	public List<TerrainSegment> terrainPrefabs = new List<TerrainSegment>(); // list of all terrain prefabs to be spawned
	private List<ITerrainSegment> _instancedTerrain = new List<ITerrainSegment>(); // list of all terrain instances that have been spawned
	
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
	private float _movementSpeedMultiplier = 1f; // used to slow down the terrain when the player dies
	[SerializeField]
	private float terrainTargetMeetThreshold = 1f;
	
	private TerrainSegment _nextSegment; // the next terrain segment to be spawned
	private bool _initialSpawningComplete = false;

	#region Unity Methods

	private void OnEnable() => GameManager.Instance.OnPlayerDie += OnPlayerDie;

	private void OnDisable() => GameManager.Instance.OnPlayerDie -= OnPlayerDie;

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
		// only move the terrain if the initial spawning is complete
		if (_initialSpawningComplete)
		{
			// create a list to store the terrain segments that will be removed
			List<ITerrainSegment> removedItems = new List<ITerrainSegment>();
			
			// loop through all the terrain segments that have been spawned
			foreach (var segment in _instancedTerrain)
			{
				// calculate the threshold buffer based on the terrain movement speed and deltaTime
				float thresholdBuffer = terrainMovementSpeed * Time.deltaTime;
				
				if (segment.IsWithinTargetThreshold( // check if the terrain segment is within the target threshold
					    segment.EndTransform.position,
					    _locationTarget - segment.GetTerrainOffsetPosition(),
					    terrainTargetMeetThreshold + thresholdBuffer))
				{
					// if true, add it to the list of segments to be removed
					removedItems.Add(segment);
				}
				else
				{
					// else, move the terrain segment
					segment.TranslateTerrain((terrainMovementSpeed * _movementSpeedMultiplier) * Time.deltaTime, Vector3.back);
				}
			}
			
			// loop through the list of terrain segments to be removed
			foreach (var removedItem in removedItems)
			{
				// remove the terrain segment from the list of spawned terrain segments
				_instancedTerrain.Remove(removedItem);
				
				// "destroy" the terrain segment
				removedItem.TerrainTransform.gameObject.SetActive(false);
			}
			
			_instancedTerrain.TrimExcess();
			removedItems.Clear();

			// only speed up the terrain if the player is alive and the speed multiplier is less than 2x
			if (_movementSpeedMultiplier <= 2f && GameManager.Instance.IsGameActive)
			{
				SpeedUpTerrainMovement();
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(_locationTarget, 1f);
	}

	#endregion
	
	private void OnPlayerDie() => StartCoroutine(SlowTerrainMovement());

	private void SpeedUpTerrainMovement()
	{
		_movementSpeedMultiplier += Time.deltaTime * 0.01f;
	}

	private IEnumerator SlowTerrainMovement()
	{
		while (_movementSpeedMultiplier > 0f)
		{
			// slow down the terrain movement
			_movementSpeedMultiplier -= Time.deltaTime;
			yield return null;
		}
		
		// disable the script when the terrain has stopped moving
		enabled = false;
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