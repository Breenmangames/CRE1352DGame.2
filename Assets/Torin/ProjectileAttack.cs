using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        DestroyWhenOffScreen();
    }
         
   /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyMovement>())
        {
            // Damages enemies upon collision via targeting their movement script then destroys the bullet
            HealthController healthController = collision.GetComponent<HealthController>();
            healthController.TakeDamage(100);
            Destroy(gameObject);
        }
    }
    */

    private void DestroyWhenOffScreen()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        // Destroys the bullet object when it leaves the boundaries of the camera
        if (screenPosition.x < 0 ||
        screenPosition.x > _camera.pixelWidth ||
        screenPosition.y < 0 ||
        screenPosition.y > _camera.pixelHeight)
        {
            Destroy(gameObject);
        }
    }
}