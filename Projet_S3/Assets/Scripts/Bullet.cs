using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage = 3;
    [SerializeField] private float speed = 5f;
    [SerializeField] private ParticleSystem explosionPrefab;

    public void setBulletSpeedAndDamage(int speed, int damage)
    {
        this.damage = damage;
        this.speed = speed;
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * (speed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.ApplyDamage(damage);
        }

        var explosion = Instantiate(explosionPrefab, other.contacts[0].point, Quaternion.identity);
        explosion.gameObject.SetActive(true);
        explosion.Play();
        Destroy(explosion.gameObject, explosion.main.duration);

        Destroy(gameObject);
    }
}
