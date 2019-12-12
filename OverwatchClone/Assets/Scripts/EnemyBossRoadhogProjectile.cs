using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossRoadhogProjectile : MonoBehaviour
{
    Vector3 direction;
    float projectileSpreadAngle = 10.05f;
    float speed = 80;
    public float projectileDamageMax = 6;
    public float projectileDamageMin = 1.8f;
    public float projectileRangeMax = 30;
    public float projectileRangeMin = 15;
    float damage;
    RaycastHit hit;
    bool moving = true;
    Vector3 origPos;
    void Start()
    {
        origPos = transform.position;
        RandomizeAngle();
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, direction, out hit, direction.magnitude)) {
            if (hit.collider.tag == "Player") {
                ProjectileHit(hit.collider);
            } else Destroy(gameObject);
        } else
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            ProjectileHit(other);
        } else Destroy(gameObject);
    }

    void ProjectileHit(Collider other) {
        CalculateDamage();
        other.GetComponent<IDamageable>().TakeDamage(damage);
        Destroy(gameObject);
    }

    void CalculateDamage() {
        var distance = Vector3.Distance(transform.position, origPos);
        if (distance >= projectileRangeMax) {
            damage = projectileDamageMin;
        }
        if (distance <= projectileRangeMin) {
            damage = projectileDamageMax;
        }
        if (distance < projectileRangeMax && distance > projectileRangeMin) {
            damage = projectileDamageMax * (projectileRangeMin / distance);
            damage = Mathf.RoundToInt(damage);
        }
    }

    void RandomizeAngle() {
        direction = Vector3.forward;
        float deviation = Random.Range(0f, projectileSpreadAngle);
        float angle = Random.Range(0f, 360f);
        direction = Quaternion.AngleAxis(deviation, Vector3.up) * direction;
        direction = Quaternion.AngleAxis(angle, Vector3.forward) * direction;
    }
}
