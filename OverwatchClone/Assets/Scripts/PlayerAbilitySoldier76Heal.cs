using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilitySoldier76Heal : MonoBehaviour
{
    public GameObject master;
    public GameObject radiusGraphic;
    public GameObject groundCheck;
    public LayerMask groundLayer;
    public float radius;
    public LayerMask playerLayers;

    public float healingAmount = 40;
    public float duration = 5;
    float timer = 0;
    float ticker = 0.2f;
    float durationTimer = 0;
    bool canHeal = true;
    bool isActive = false;
    RaycastHit hit;
    Vector3 location;


    private void FixedUpdate()
    {
        bool touchDown = false;
        if (Physics.Raycast(groundCheck.transform.position, Vector3.down, out hit, 0.5f, groundLayer))
        {
            isActive = true;
            location = hit.point;
        }
        if (!isActive)
        {
            transform.position += Vector3.down * Time.deltaTime * 12;
            radiusGraphic.SetActive(false);
        }
        if (isActive)
        {
            if (!touchDown)
            {
                gameObject.transform.position = location;
            }
            radiusGraphic.SetActive(true);
            durationTimer += Time.deltaTime;
            if (durationTimer >= duration)
            {
                Destroy(gameObject);
            }
        }
        if (canHeal) {
            Collider[] healTargets = Physics.OverlapSphere(transform.position, radius, playerLayers);
            foreach (Collider players in healTargets) {
                players.GetComponent<PlayerHealthManager>().ReceiveHealth(healingAmount / 5, master);
                canHeal = false;
            }
        }
        if (!canHeal) {
            timer += Time.deltaTime;
            if (timer >= ticker) {
                timer -= ticker;
                canHeal = true;
            }
        }
    }
}
