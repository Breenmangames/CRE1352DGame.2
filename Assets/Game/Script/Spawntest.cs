using UnityEngine;
using UnityEngine.Tilemaps;

public class Spawntest : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab; // Reference to the enemy prefab that will be spawned during encounters
    [SerializeField] private float _minimumSpawnTime = 2f; // Minimum time in seconds between encounter checks, allowing us to control how frequently encounters can occur
    [SerializeField] private float _maximumSpawnTime = 5f; // Maximum time in seconds between encounter checks, allowing us to control how frequently encounters can occur
    [SerializeField] private GameObject player; // Reference to the player GameObject, used to determine the player's position for encounter checks and spawning enemies near the player
    [SerializeField] private float detectionRadius = 0.1f; // how close to a tile center to consider "on the tile"
    [SerializeField, Range(0, 100)] private int encounterChancePercent = 10;

    public LayerMask grassLayer; // LayerMask to specify which layer(s) are considered "grass" for encounter checks, allowing us to determine where encounters can occur based on the player's proximity to grass tiles

    private float _timeUntilSpawn; // Timer to track the time until the next encounter check, allowing us to control how frequently encounters can occur

    public Vector3 spawnPos; // Public variable to store the spawn position for debugging purposes, allowing us to see where enemies will spawn during encounters in the editor

    private void Awake()
    {
        SetTimeUntilSpawn(); // Initialize the timer for the first encounter check when the script is first loaded, ensuring that encounters can start occurring after a random interval between the minimum and maximum spawn times
    }

    private void Update()
    {
        _timeUntilSpawn -= Time.deltaTime;

        if (_timeUntilSpawn <= 0f)
        {
            Vector3 spawnPos;
            if (TryGetEncounterSpawnPosition(out spawnPos) && Random.Range(1, 101) <= encounterChancePercent) // Check if an encounter spawn position can be determined and if a random roll falls within the encounter chance percentage, allowing us to control the frequency of encounters when the player is near grass tiles
            {
                Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
               
            }

            SetTimeUntilSpawn(); // Reset the timer for the next encounter check, ensuring that encounters can continue to occur at random intervals between the minimum and maximum spawn times
        }
    }

    private void SetTimeUntilSpawn()
    {
        _timeUntilSpawn = Random.Range(_minimumSpawnTime, _maximumSpawnTime);
    }

    
    public bool TryGetEncounterSpawnPosition(out Vector3 spawnPosition)  // (AI assisted here to help fix the enemy spawning in center of map, only the spawn position part of code is assisted not the detection part) Method to determine a valid spawn position for an encounter when the player is near a grass tile, returning true and the spawn position if a valid location is found, allowing us to control where enemies can spawn during encounters based on the player's proximity to grass tiles
    {
        spawnPosition = Vector3.zero;// Initialize the spawn position to zero in case a valid location is not found, ensuring that the output variable is always assigned a value even if the method returns fals
        if (player == null) 
        {
            return false; 
        }

        Vector2 playerWorldPos = player.transform.position; // Get the player's current world position, which will be used to check for nearby grass tiles and determine potential spawn locations for encounters
        Collider2D hit = Physics2D.OverlapCircle(playerWorldPos, detectionRadius, grassLayer); //Check for a collider on the specified grass layer within the detection radius around the player's position, allowing us to determine if the player is near a grass tile where encounters can occur
        if (hit == null) return false;

      
        float angle = Random.Range(0f, Mathf.PI * 2f); // Generate a random angle in radians to determine the direction from the player to the potential spawn position, allowing us to spawn enemies in a random direction around the player when an encounter occurs
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * detectionRadius; // Calculate an offset from the player's position based on the random angle and the detection radius, which will be used to determine a candidate spawn position for the encounter around the player when they are near a grass tile
        Vector2 candidateWorldPos = playerWorldPos + offset; // Calculate a candidate spawn position by adding the offset to the player's world position, which will be used to determine if there is a valid tile at that location for spawning an enemy during an encounter when the player is near a grass tile


        Tilemap tilemap = hit.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            Vector3Int cell = tilemap.WorldToCell(candidateWorldPos);
            spawnPosition = tilemap.GetCellCenterWorld(cell);
            return true;
        }
        spawnPosition = (Vector3)candidateWorldPos; // Convert to Vector3 for consistency
        return true;
    }

    
    private void OnDrawGizmosSelected() // Draws a wire sphere in the editor to visualize the detection radius around the player, helping us see where encounters can occur when the player is near grass tiles
    {
        if (player != null) // Check if the player reference is set before trying to draw the gizmo, preventing potential errors in the editor if the reference is missing
        {
            Gizmos.color = Color.green; // Set the gizmo color to green for better visibility in the editor, making it easier to see the detection radius around the player
            Gizmos.DrawWireSphere(player.transform.position, detectionRadius); // Draw a wire sphere at the player's position with the specified detection radius, visualizing the area where encounters can occur when the player is near grass tiles
        }
    }
}
