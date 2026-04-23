using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestProjectile : MonoBehaviour
{
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private float _bulletSpeed;
    [SerializeField]
    private Transform _gunOffset;
    [SerializeField]
    private float _timeBetweenShots;

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
        Quaternion aimRotation = Quaternion.FromToRotation(Vector2.up, _aimDirection);
        GameObject bullet = Instantiate(_bulletPrefab, _gunOffset.position, aimRotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        rb.linearVelocity = _bulletSpeed * _aimDirection;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
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