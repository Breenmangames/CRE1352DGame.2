using UnityEngine;
using UnityEngine.Tilemaps;

public class Spawntest : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _minimumSpawnTime = 2f;
    [SerializeField] private float _maximumSpawnTime = 5f;
    [SerializeField] private GameObject player;
    [SerializeField] private float detectionRadius = 0.1f; // how close to a tile center to consider "on the tile"
    [SerializeField, Range(0, 100)] private int encounterChancePercent = 10;

    public LayerMask grassLayer;

    private float _timeUntilSpawn;

    private void Awake()
    {
        SetTimeUntilSpawn();
    }

    private void Update()
    {
        _timeUntilSpawn -= Time.deltaTime;

        if (_timeUntilSpawn <= 0f)
        {
            Vector3 spawnPos;
            if (TryGetEncounterSpawnPosition(out spawnPos) && Random.Range(1, 101) <= encounterChancePercent)
            {
                Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
                Debug.Log($"Spawned enemy at {spawnPos}");
            }

            SetTimeUntilSpawn();
        }
    }

    private void SetTimeUntilSpawn()
    {
        _timeUntilSpawn = Random.Range(_minimumSpawnTime, _maximumSpawnTime);
    }

    // Tries to determine a good spawn position when the player is on/near a grass tile.
    // Returns true and the world position to spawn if an encounter location is found.
    private bool TryGetEncounterSpawnPosition(out Vector3 spawnPosition)
    {
        spawnPosition = Vector3.zero;
        if (player == null) return false;

        Vector2 playerWorldPos = player.transform.position;

        // Check if player is overlapping the grass layer (small radius around player's position)
        Collider2D hit = Physics2D.OverlapCircle(playerWorldPos, detectionRadius, grassLayer);
        if (hit == null) return false;

        // If the collider belongs to a Tilemap, snap spawn position to the tile center
        Tilemap tilemap = hit.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            Vector3Int cell = tilemap.WorldToCell(playerWorldPos);
            spawnPosition = tilemap.GetCellCenterWorld(cell);
            return true;
        }

        // Fallback: use the player's position
        spawnPosition = player.transform.position;
        return true;
    }

    // Optional: visualize the detection check in the editor
    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.transform.position, detectionRadius);
        }
    }
}
