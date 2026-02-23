using UnityEngine;

public class Spawmer : MonoBehaviour
{   
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private float _minimumSpawnTime;
    [SerializeField]
    private float _maximumSpawnTime;

    private float _spawnTime;

    private void Awake()
    {
        SetTimeUntilSpawn();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _spawnTime = Time.deltaTime;

        if( _spawnTime < _minimumSpawnTime)
        {
            Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
            SetTimeUntilSpawn();
        }
    }

    void SetTimeUntilSpawn()
    {
        _spawnTime = Random.Range(_minimumSpawnTime, _maximumSpawnTime);

    }
}
