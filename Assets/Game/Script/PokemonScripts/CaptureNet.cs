using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class CaptureNet: MonoBehaviour
{
        public CaptureItem captureItem; // Reference to the CaptureItem component, which should be on the same GameObject or assigned in the Inspector
        ProjectileShoot projectileShoot; // Reference to the ProjectileShoot component, which should be on the same GameObject or assigned in the Inspector
        [Header("References")] // These references should be assigned in the Inspector
        public GameObject captureItemPrefab; // Prefab of the capture item to be thrown, should have a CaptureItem component
    public Transform throwOrigin; // Point from which the capture item will be thrown, can be the player position or a specific child transform

    private Vector2 _aimDirection2 = Vector2.up; // Initial aim direction, will be updated based on mouse position

    [Header("Input")]
        public KeyCode throwKey = KeyCode.T;
        public KeyCode deployKey = KeyCode.G;

        private MonsterInventory _inventory; // Reference to the MonsterInventory component, which should be on the same GameObject or assigned in the Inspector

    private void Start()
        {   
            captureItem = captureItemPrefab.GetComponent<CaptureItem>();
            _inventory = GetComponent<MonsterInventory>();
            projectileShoot = GetComponent<ProjectileShoot>();
    }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                ThrowCaptureItem(); // Call the method to throw the capture item when the throw key is pressed

        if (Input.GetKeyDown(KeyCode.G))
                DeployFirst(); // Call the method to deploy the first captured enemy when the deploy key is pressed

        Update2AimDirection(); // Update the aim direction based on the mouse position every frame, so the capture item will be thrown towards the mouse cursor
    }


    public void Update2AimDirection() // This method calculates the aim direction based on the mouse position relative to the throw origin
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); // Convert mouse position from screen space to world space
        mouseWorld.z = 0f; // Ensure the z-coordinate is zero since we're working in 2D

        Vector2 rawDirection = (mouseWorld - throwOrigin.position); // Calculate the raw direction vector from the throw origin to the mouse position

        if (rawDirection.sqrMagnitude > 0.001f) // Check if the raw direction is significant enough to avoid issues with very small vectors
        {
            _aimDirection2 = rawDirection.normalized; // Normalize the raw direction to get a unit vector representing the aim direction
        }

    }
    private void ThrowCaptureItem()
        {
            if (captureItemPrefab == null)
            {
                
                return;
            }

         Quaternion aimRotation = Quaternion.FromToRotation(Vector2.up, _aimDirection2); // Calculate the rotation needed to align the capture item with the aim direction
        Transform origin = throwOrigin != null ? throwOrigin : transform; // Use the specified throw origin if assigned, otherwise default to the current GameObject's transform
        GameObject go = Instantiate(captureItemPrefab, origin.position, origin.rotation); // Instantiate the capture item prefab at the throw origin's position and rotation
        var item = go.GetComponent<CaptureItem>(); // Get the CaptureItem component from the instantiated object to call its ItemThrow method
        item?.ItemThrow(_aimDirection2); // Call the ItemThrow method on the CaptureItem component, passing in the aim direction to throw the item towards the mouse cursor
    }

        private void DeployFirst() // This method checks the inventory for captured enemies and deploys the first one that is not currently deployed
    {
            if (_inventory == null || _inventory.capturedEnemies.Count == 0)
            {
                
                return;
            }

            for (int i = 0; i < _inventory.capturedEnemies.Count; i++) // Loop through the captured enemies in the inventory
        {
                if (!_inventory.capturedEnemies[i].isDeployed)
                {
                    _inventory.Deploy(i); // Call the Deploy method on the inventory to deploy the captured enemy at index i
                return;
                }
            }
        }
 }

