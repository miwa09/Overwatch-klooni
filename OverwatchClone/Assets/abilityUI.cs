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

    private void Update() {
        if (!disabled && !cooldown) {
            fill.color = regularFill;
            border.color = regularBorder;
        }
        if (disabled) {
            fill.color = disabledFill;
            border.color = disabledBorder;
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
}
