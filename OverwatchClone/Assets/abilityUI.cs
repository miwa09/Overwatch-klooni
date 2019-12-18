using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class abilityUI : MonoBehaviour
{
    public Color disabledFill;
    public Color regularFill;
    public Color disabledBorder;
    public Color regularBorder;
    public Image fill;
    public Image border;
    public bool disabled;
    public bool cooldown;
    public float cooldownAmount;
    float timer;
    public bool hasCharges;
    public int maxChargeAmount;
    int chargeAmount;
    bool runOnce;

    private void Start() {
        chargeAmount = maxChargeAmount;
    }
    private void Update() {
        if (!disabled && !cooldown) {
            fill.color = regularFill;
            border.color = regularBorder;
        }
        if (disabled) {
            fill.color = disabledFill;
            border.color = disabledBorder;
        }
        if (hasCharges && chargeAmount < maxChargeAmount) {
            timer += Time.deltaTime;
            fill.fillAmount = timer / cooldownAmount;
            if (timer >= cooldownAmount) {
                timer -= cooldownAmount;
                chargeAmount++;
            }
            if (chargeAmount <= 0) {
                disabled = true;
            } else disabled = false;
        }
        if (cooldown) {
            disabled = true;
            timer += Time.deltaTime;
            fill.fillAmount = timer / cooldownAmount;
            if (timer >= cooldownAmount) {
                timer = 0;
                cooldown = false;
            }
        }
    }

    public void UseCharge() {
        chargeAmount--;
    }
}
