using UnityEngine;
using UnityEngine.Tilemaps;

public class Spawmer : MonoBehaviour
{   
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private float _minimumSpawnTime;
    [SerializeField]
    private float _maximumSpawnTime;
    [SerializeField]
    private GameObject player;

    public LayerMask playerLayer;
    public LayerMask grassLayer;

    private float _spawnTime;

    private void Awake()
    {
        SetTimeUntilSpawn();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void FixedUpdate()
    {
        CheckForEncounters();
    }
    // Update is called once per frame
    void Update()
    {
        _spawnTime = Time.deltaTime;

        if( _spawnTime < _minimumSpawnTime)
        {
            Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
            //gameObject.GetComponent<Tilemap>;
            SetTimeUntilSpawn();
        }
        
    }

    void SetTimeUntilSpawn()
    {
        _spawnTime = Random.Range(_minimumSpawnTime, _maximumSpawnTime);

    }


    private void CheckForEncounters()
    {
        Collider2D check = Physics2D.OverlapCircle(transform.position, 20.2f, grassLayer);
        if (check != null)
        {
            Debug.Log(check.transform.position);
            if (Random.Range(1, 101) <= 10) // 10% chance
            {
                Debug.Log("A wild enemy appears!");
                // Trigger encounter logic here
            }
        }
    }
}
