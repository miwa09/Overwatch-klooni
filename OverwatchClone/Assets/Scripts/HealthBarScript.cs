using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public PlayerHealthManager hpScript;
    public float health;
    public float maxHealth;
    public Image healthBar;

    private void Start() {
        maxHealth = hpScript.maxHealth;
        health = hpScript.health;
    }

    private void Update() {
        health = hpScript.health;
        healthBar.fillAmount = health / maxHealth;
    }
}
