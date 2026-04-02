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

    CaptureNet captureNet;

    private bool _fireContinuously;
    private bool _fireSingle;
    private float _lastFireTime;
    private Vector2 _aimDirection = Vector2.up;
    private Vector2 knockbackForce;

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

    public void UpdateAimDirection()
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
        
        EnemyHealth enemyHP = other.GetComponentInParent<EnemyHealth>();

        if (enemyHP != null)
        {
            enemyHP.TakeDamage(_damage);

            Rigidbody2D enemyRb = other.GetComponentInParent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
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