using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttack : MonoBehaviour
{
    public float velocity = 10;
    Rigidbody rig;
    public Transform target;
    public float damage = 25;
    public float explosionRadius = 2;
    public float explosionDamageMax = 60;
    public float explosionDamageMin = 20;
    Vector3 hit;
    public LayerMask damageMask;

    private void Start() {
        rig = GetComponent<Rigidbody>();
        rig.AddForce(HitTargetByAngle(transform.position, target.position, Physics.gravity, 45), ForceMode.Impulse);
    }



    private void OnCollisionEnter(Collision collision) {
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);
        int i = 0;

        if (hitColliders.Length > 0) {
            while (i < hitColliders.Length) {
                float distance = Vector3.Distance(hit, hitColliders[i].gameObject.transform.position) - 0.5f;
                if (distance <= 0) {
                    distance = 0;
                }
                float explosionDamage = explosionDamageMax / distance;
                if (explosionDamage <= 60) {
                    explosionDamage = 60;
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
