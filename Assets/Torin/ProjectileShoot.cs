using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileShoot : MonoBehaviour
{
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private float _bulletSpeed;
    [SerializeField]
    private Transform _gunOffset;
    [SerializeField]
    private float _timeBetweenShots;
    [SerializeField]
    private int _damage = 25;
    public LayerMask EnemyLayer;

    private bool _fireContinuously;
    private bool _fireSingle;
    private float _lastFireTime;
    private Vector2 _aimDirection = Vector2.up;

    void Update()
    {
        UpdateAimDirection();

        if (_fireContinuously || _fireSingle)
        {
            float timeSinceLastFire = Time.time - _lastFireTime;
            if (timeSinceLastFire >= _timeBetweenShots)
            {
                FireBullet();
                _lastFireTime = Time.time;
                _fireSingle = false;
            }
        }
    }

    private void UpdateAimDirection()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;

        Vector2 rawDirection = (mouseWorld - _gunOffset.position);

        if (rawDirection.sqrMagnitude > 0.001f)
        {
            _aimDirection = rawDirection.normalized;
        }

    }

    public void FireBullet()
    {
        Debug.Log($"Firing in direction: {_aimDirection}");

        Quaternion aimRotation = Quaternion.FromToRotation(Vector2.up, _aimDirection);
        GameObject bullet = Instantiate(_bulletPrefab, _gunOffset.position, aimRotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        Debug.Log($"Rigidbody found: {rb != null}");
        rb.linearVelocity = _bulletSpeed * _aimDirection;
        Debug.Log($"Velocity set to: {rb.linearVelocity}");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Projectile collision with: " + other.gameObject.name +
                  " | Layer: " + LayerMask.LayerToName(other.gameObject.layer) +
                  " | Tag: " + other.gameObject.tag);



        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(_damage);
        }

        Destroy(gameObject);
    }

    private void OnAttack(InputValue inputValue)
    {
        _fireContinuously = inputValue.isPressed;
        if (inputValue.isPressed)
        {
            _fireSingle = true;
        }
    }
}