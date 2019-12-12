using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossJunkratBomb : MonoBehaviour
{
    public float velocity = 10;
    Rigidbody rig;
    public Transform target;
    public float damage = 130;
    public float explosionRadius = 2;
    public float explosionDamageMax = 80;
    public float explosionDamageMin = 10;
    public LayerMask damageMask;
    int bounce = 0;

    private void Start() {
        rig = GetComponent<Rigidbody>();
        rig.AddForce(HitTargetByAngle(transform.position, target.position, Physics.gravity, 45), ForceMode.Impulse);
    }

    private void Update() {
        if (bounce >= 2) {
            Explosion();
        }
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        bounce++;
    }

    private void OnTriggerEnter(Collider other) {

    }

    void Explosion() {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);
        int i = 0;
        if (hitColliders.Length > 0) {
            while (i < hitColliders.Length) {
                float distance = Vector3.Distance(transform.position, hitColliders[i].gameObject.transform.position) - 0.5f;
                if (distance < 0) {
                    distance = 0;
                }
                print(distance);
                float explosionDamage = explosionDamageMax / distance;
                if (explosionDamage <= explosionDamageMin) {
                    explosionDamage = explosionDamageMin;
                }
                if (explosionDamage >= explosionDamageMax) {
                    explosionDamage = explosionDamageMax;
                }
                if (hitColliders[i].gameObject.tag == "Player") {
                    hitColliders[i].gameObject.GetComponent<IDamageable>().TakeDamage(explosionDamage);
                    hitColliders[i].gameObject.GetComponent<Rigidbody>().AddExplosionForce(150f, gameObject.transform.position, 4, 1);
                }
                i++;
            }
        }
        Destroy(gameObject);
    }

    public static Vector3 HitTargetByAngle(Vector3 startPosition, Vector3 targetPosition, Vector3 gravityBase, float limitAngle) {
        if (limitAngle == 90 || limitAngle == -90) {
            return Vector3.zero;
        }

        Vector3 AtoB = targetPosition - startPosition;
        Vector3 horizontal = GetHorizontalVector(AtoB, gravityBase);
        float horizontalDistance = horizontal.magnitude;
        Vector3 vertical = GetVerticalVector(AtoB, gravityBase);
        float verticalDistance = vertical.magnitude * Mathf.Sign(Vector3.Dot(vertical, -gravityBase));

        float angleX = Mathf.Cos(Mathf.Deg2Rad * limitAngle);
        float angleY = Mathf.Sin(Mathf.Deg2Rad * limitAngle);

        float gravityMag = gravityBase.magnitude;

        if (verticalDistance / horizontalDistance > angleY / angleX) {
            return Vector3.zero;
        }

        float destSpeed = (1 / Mathf.Cos(Mathf.Deg2Rad * limitAngle)) * Mathf.Sqrt((0.5f * gravityMag * horizontalDistance * horizontalDistance) / ((horizontalDistance * Mathf.Tan(Mathf.Deg2Rad * limitAngle)) - verticalDistance));
        Vector3 launch = ((horizontal.normalized * angleX) - (gravityBase.normalized * angleY)) * destSpeed;
        return launch;
    }
    public static Vector3 GetHorizontalVector(Vector3 AtoB, Vector3 gravityBase) {
        Vector3 output;
        Vector3 perpendicular = Vector3.Cross(AtoB, gravityBase);
        perpendicular = Vector3.Cross(gravityBase, perpendicular);
        output = Vector3.Project(AtoB, perpendicular);
        return output;
    }

    public static Vector3 GetVerticalVector(Vector3 AtoB, Vector3 gravityBase) {
        Vector3 output;
        output = Vector3.Project(AtoB, gravityBase);
        return output;
    }
}
