using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaptureNet: MonoBehaviour
{

    [Header("References")]
     public GameObject captureItemPrefab;
     public Transform throwOrigin;      

        [Header("Input")]
        public KeyCode throwKey = KeyCode.Q;
        public KeyCode deployKey = KeyCode.E;

        private MonsterInventory _inventory;

        private void Start()
        {
            _inventory = GetComponent<MonsterInventory>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
                ThrowCaptureItem();

            if (Input.GetKeyDown(KeyCode.Q))
                DeployFirst();
        }

        private void ThrowCaptureItem()
        {
            if (captureItemPrefab == null)
            {
                Debug.LogWarning("[Thrower] No capture item prefab assigned.");
                return;
            }

            Transform origin = throwOrigin != null ? throwOrigin : transform;
            GameObject go = Instantiate(captureItemPrefab, origin.position, origin.rotation);
            var item = go.GetComponent<CaptureItem>();
            item?.Throw(origin);
        }

        private void DeployFirst()
        {
            if (_inventory == null || _inventory.capturedEnemies.Count == 0)
            {
                Debug.Log("[Thrower] No captured enemies to deploy.");
                return;
            }

            // Deploy the first non-deployed enemy
            for (int i = 0; i < _inventory.capturedEnemies.Count; i++)
            {
                if (!_inventory.capturedEnemies[i].isDeployed)
                {
                    _inventory.Deploy(i);
                    return;
                }
            }
        }
 }

