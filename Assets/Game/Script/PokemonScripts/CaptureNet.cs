using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class CaptureNet: MonoBehaviour
{
        public CaptureItem captureItem;
        ProjectileShoot projectileShoot;
    [Header("References")]
        public GameObject captureItemPrefab;
        public Transform throwOrigin;

    private Vector2 _aimDirection2 = Vector2.up;

    [Header("Input")]
        public KeyCode throwKey = KeyCode.T;
        public KeyCode deployKey = KeyCode.G;

        private MonsterInventory _inventory;

        private void Start()
        {   
            captureItem = captureItemPrefab.GetComponent<CaptureItem>();
            _inventory = GetComponent<MonsterInventory>();
            projectileShoot = GetComponent<ProjectileShoot>();
    }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                ThrowCaptureItem();

            if (Input.GetKeyDown(KeyCode.G))
                DeployFirst();

        Update2AimDirection();
    }


    public void Update2AimDirection()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;

        Vector2 rawDirection = (mouseWorld - throwOrigin.position);

        if (rawDirection.sqrMagnitude > 0.001f)
        {
            _aimDirection2 = rawDirection.normalized;
        }

    }
    private void ThrowCaptureItem()
        {
            if (captureItemPrefab == null)
            {
                
                return;
            }

         Quaternion aimRotation = Quaternion.FromToRotation(Vector2.up, _aimDirection2);
        Transform origin = throwOrigin != null ? throwOrigin : transform;
            GameObject go = Instantiate(captureItemPrefab, origin.position, origin.rotation);
            var item = go.GetComponent<CaptureItem>();
            item?.ItemThrow(_aimDirection2);
        }

        private void DeployFirst()
        {
            if (_inventory == null || _inventory.capturedEnemies.Count == 0)
            {
                
                return;
            }

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

