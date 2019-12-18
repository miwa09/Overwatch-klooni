using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilitiesBrigitte : MonoBehaviour, IUltCharge
{
    public string inputPrefix = "P2";
    public bool disabled = false;
    public PlayerBrigitteMelee meleeScript;

    //Passive
    public float passiveCooldown = 1;
    float passiveCDTimer;
    bool passiveCD = false;
    bool passiveHealOn = false;
    public int passiveDuration = 6;
    public float passiveSelfHeal = 65f;
    public float passiveTeamHeal = 130f;
    List<Collider> passiveHealList = new List<Collider>();
    public float passiveRadius = 20;

    // Heal
    public int healMaxCharges = 3;
    int healCharges;
    public float healAmount = 120;
    public float maxTempArmor = 75;
    public float healRange = 30;
    public int healDuration = 2;
    public int healsPerTick = 5;
    public float tempArmorDuration = 5;
    public int chargeRestoreTime = 6;
    float chargeRestoreTicker = 1;
    int chargeRestoreCounter;
    float chargeRestoreTimer = 0;
    Collider healTarget;
    public Transform cameraPos;
    public LayerMask player1Layer;
    public LayerMask groundlayer;
    RaycastHit hit;
    public GameObject healMarker;
    public Camera cam;
    public Text healUI;
    public Text healChargeUI;
    public float healRecoveryDuration = 0.25f;
    float healRecoveryTimer = 0;
    bool healRecovery = false;
    public abilityUI healIcon;

    //Whipshot
    public GameObject whipshotHitbox;
    public float whipshotSpeed = 80;
    public float whipshotRange = 20;
    public float whipshotDamage = 70;
    public bool isMoving = false;
    public bool isReturning = false;
    public int whipshotCooldown = 4;
    float whipshotTimer = 0;
    float whipshotTicker = 1;
    public int whipshotCounter = 0;
    bool whipshotCD = false;
    public Text whipshotUI;

    //Shield
    public PlayerMover moveScript;
    public GameObject gO_shieldCamera;
    public GameObject gO_regularCamera;
    public GameObject gO_shield;
    public Animation anim_shieldOpening;
    public Text ui_shield;
    public Text ui_shieldHPInactive;
    public Text ui_shieldHPActive;
    float shieldInput;
    Color ui_shieldOKColor;
    bool playShieldAnimOnce;
    float normalMoveSpeed;
    float shieldMoveSpeed;
    public float shieldMaxHealth = 200;
    public float shieldHealth;
    public float shieldRegenPerSec = 100;
    float shieldTimer = 0;
    float shieldTicker = 0.05f;
    float shieldRegenTimer = 0;
    bool isShieldRegening = false;
    public float shieldRegenDelay = 2;
    public int shieldBreakCD = 3;
    float shieldBreakTimer = 0;
    float shieldBreakTicker = 1;
    int shieldBreakCounter = 0;
    bool shieldBroken = false;
    public bool shieldActive = false;

    //Shieldbash
    public LayerMask enemyLayer;
    public Text ui_sbability;
    float shieldBashInput;
    Rigidbody rig;
    public float sbDamage = 5;
    public float sbStunDuration = 0.75f;
    public int sbCooldownDuration = 7;
    float sbCooldownTimer = 0;
    int sbCooldownCounter = 0;
    bool sbCooldown = false;
    public float sbRange = 6;
    bool sbActive = false;
    float sbMoveTimer = 0;
    public float sbMoveDuration = 0.1f;

    //Ultimate
    public LayerMask playerLayers;
    public Text ui_ult;
    public float ultCharge;
    public float maxUltCharge = 2800;
    float ultChargeTimer = 0;
    float ultChargeTicker = 1;
    public float ultMoveSpeedMultiplier = 1.3f;
    float ultMoveSpeed;
    public float ultDuration = 10;
    float ultTimer = 0;
    public float ultTempArmorPerTick = 15;
    float ultArmorTimer = 0;
    float ultArmorTicker = 0.5f;
    public float ultMaxTempArmor = 100;
    public int ultTempArmorDuration = 30;
    public float ultRadius = 8.5f;
    public bool ultReady = false;
    bool ultOn = false;
    float ultShieldMoveSpeed;
    public float ultRecoveryDuration = 0.7f;
    float ultRecoveryTimer = 0;
    bool ultDisable = false;
    
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        ui_shieldOKColor = ui_shieldHPInactive.color;
        normalMoveSpeed = moveScript.movementSpeed;
        shieldMoveSpeed = normalMoveSpeed * 0.7f;
        ultShieldMoveSpeed = shieldMoveSpeed * ultMoveSpeedMultiplier;
        ultMoveSpeed = moveScript.movementSpeed * ultMoveSpeedMultiplier;
        shieldHealth = shieldMaxHealth;
        healCharges = healMaxCharges;
        chargeRestoreCounter = chargeRestoreTime;
    }

    void Update()
    {
        UI();
        
        HealChargeRestore();
        ShieldRegen();
        
        if (passiveCD) {
            passiveCDTimer += Time.deltaTime;
            if (passiveCDTimer >= passiveCooldown) {
                passiveCDTimer = 0;
                passiveCD = false;
            }
        }
        if (!disabled) {
            HealGetTarget();
        }
        if (disabled) {
            healTarget = null;
            healMarker.SetActive(false);
        }
        if (canHeal() && !ultDisable && !healRecovery) {
            AbilityHeal();
        }
        if (healRecovery) {
            HealRecovery();
        }
        if (!whipshotCD && !disabled && !ultDisable && !healRecovery) {
            WhipshotMove();
            AbilityWhipshot();
        }
        if (whipshotCD) {
            WhipshotCooldown();
        }
        if ((Input.GetButton(inputPrefix + "Ability1") && !shieldBroken && !healRecovery) || (shieldInput > 0.9f && !shieldBroken && !healRecovery)) {
            AbilityShield();
        }
        if ((Input.GetButtonUp(inputPrefix + "Ability1") && shieldActive) || (shieldInput < 0.9f && shieldActive)) {
            AbilityShieldEnd();
        }
        if (shieldBroken) {
            ShieldBroken();
        }
        if (!sbCooldown && shieldActive && !sbActive) {
            AbilityShieldBash();
        }
        if (sbCooldown) {
            ShieldBashCooldown();
        }
        if (sbActive) {
            ShieldBashMove();
        }
        if (disabled || healRecovery || ultDisable) {
            meleeScript.disabled = true;
        }
        if (!disabled && !healRecovery && !ultDisable) {
            meleeScript.disabled = false;
        }
        if (!ultReady && !ultOn) {
            ultChargeTimer += Time.deltaTime;
            if (ultChargeTimer >= ultChargeTicker) {
                ultChargeTimer -= ultChargeTicker;
                ultCharge += 5;
            }
            if (ultCharge >= maxUltCharge) {
                ultCharge = maxUltCharge;
                ultReady = true;
            }
        }
        if (ultReady) {
            AbilityUlt();
        }
        if (ultOn) {
            UltActive();
        }
        if (ultDisable) {
            UltRecovery();
        }
        shieldInput = Input.GetAxis(inputPrefix + "Ability1");
        shieldBashInput = Input.GetAxis(inputPrefix + "PrimaryFire");
    }

    public void AbilityPassive() {
        if (!passiveHealOn) {
            passiveHealOn = true;
        }
        if (passiveHealOn && passiveCD) {
            return;
        }
        if (passiveHealOn && !passiveCD) {
            Collider[] hitList = Physics.OverlapSphere(transform.position, passiveRadius);
            foreach (Collider obj in hitList) {
                if(obj.tag == "Player" && !passiveHealList.Contains(obj)) {
                    passiveHealList.Add(obj);
                    if (obj.GetComponent<PlayerBrigitteShield>() != null) {
                        passiveHealList.Remove(obj);
                    }
                }
            }
            foreach (Collider player in passiveHealList) {
                if (player.gameObject == gameObject) {
                    GetComponent<PlayerHealthManager>().StartHealOverTime(passiveSelfHeal, passiveDuration, false, 0, gameObject);
                } else player.GetComponent<PlayerHealthManager>().StartHealOverTime(passiveTeamHeal, passiveDuration, false, 0, gameObject);
            }
            passiveHealList.Clear();
            passiveCD = true;
            return;
        }
    }

    //Heal code

    bool canHeal() {
        if (healTarget != null) {
            if (healCharges > 0) {
                return true;
            } else return false;
        } else return false;
    }
    void AbilityHeal() {
        if (Input.GetButtonDown(inputPrefix + "Ability3")) {
            healIcon.UseCharge();
            healCharges--;
            var overheal = 0f;
            var targetHM = healTarget.transform.parent.GetComponent<PlayerHealthManager>();
            if (targetHM.maxHealth - targetHM.health < healAmount) {
                overheal = healAmount - (targetHM.maxHealth - targetHM.health);
                if (overheal > maxTempArmor) {
                    overheal = maxTempArmor;
                }
            }
            targetHM.StartHealOverTime(healAmount, healDuration, true, overheal, gameObject);
            healRecovery = true;
        }
    }
    
    void HealChargeRestore() {
        if (healCharges < healMaxCharges) {
            chargeRestoreTimer += Time.deltaTime;
            if (chargeRestoreTimer >= chargeRestoreTicker) {
                chargeRestoreTimer -= chargeRestoreTicker;
                chargeRestoreCounter--;
            }
            if (chargeRestoreCounter <= 0) {
                healCharges++;
                chargeRestoreCounter = chargeRestoreTime;
                return;
            }
        }
        if (healCharges >= healMaxCharges) {
            healCharges = healMaxCharges;
        }
    }

    void HealGetTarget() {
        if (healTarget == null) {
            var direction = cameraPos.forward;
            if (Physics.Raycast(cameraPos.position, direction, out hit, healRange, player1Layer)) {
                if (!Physics.Raycast(cameraPos.position, direction, hit.distance, groundlayer)) {
                    healTarget = hit.collider;
                }
            }
            healMarker.SetActive(false);
        }
        if (healTarget != null) {
            HealCheckTarget();
            if (healTarget == null) {
                return;
            }
            Vector3 healMarkerScreenPos = cam.WorldToScreenPoint(healTarget.transform.position);
            healMarker.transform.position = healMarkerScreenPos;
            healMarker.SetActive(true);
        }
    }

    void HealCheckTarget() {
        var direction = cameraPos.forward;
        if (healTarget.GetComponentInParent<PlayerHealthManager>().hasDied) {
            healTarget = null;
            return;
        }
        if (!Physics.Raycast(cameraPos.position, direction, out hit, healRange, player1Layer)){
            healTarget = null;
            return;
        }
        if (Physics.Raycast(cameraPos.position, direction, hit.distance, groundlayer)) {
            healTarget = null;
            return;
        }
    }

    void HealRecovery() {
        healRecoveryTimer += Time.deltaTime;
        if (healRecoveryTimer >= healRecoveryDuration) {
            healRecoveryTimer = 0;
            healRecovery = false;
        }
    }

    //Whipshot code
 
    void AbilityWhipshot() {
        if(Input.GetButtonDown(inputPrefix + "Ability2") && !isMoving) {
            isMoving = true;
        }
    }

    void WhipshotMove() {
            if (isMoving) {
            whipshotHitbox.SetActive(true);
            if (!isReturning) {
                    whipshotHitbox.transform.position += cameraPos.forward * whipshotSpeed * Time.deltaTime;
                    if (Vector3.Distance(whipshotHitbox.transform.position, transform.position) > whipshotRange) {
                        isReturning = true;
                    }
                }
                if (isReturning) {
                    whipshotHitbox.transform.position = Vector3.MoveTowards(whipshotHitbox.transform.position, transform.position, (whipshotSpeed * 0.75f) * Time.deltaTime);
                    if (Vector3.Distance(whipshotHitbox.transform.position, transform.position) < 1.5) {
                        isReturning = false;
                        isMoving = false;
                    whipshotHitbox.transform.position = cameraPos.transform.position;
                    whipshotHitbox.SetActive(false);
                    whipshotHitbox.GetComponent<PlayerBrigitteWhipshot>().damageOnce = true;
                    whipshotCD = true;
                }
                }
        }
    }

    void WhipshotCooldown() {
        whipshotTimer += Time.deltaTime;
        if (whipshotTimer >= whipshotTicker) {
            whipshotTimer -= whipshotTicker;
            whipshotCounter++;
        }
        if (whipshotCounter >= whipshotCooldown) {
            whipshotCD = false;
            whipshotCounter = 0;
        }
    }

    //Shield Code
    void AbilityShield() {
        isShieldRegening = false;
        shieldRegenTimer = 0;
        gO_shieldCamera.SetActive(true);
        gO_regularCamera.SetActive(false);
        GetComponent<PlayerHealthManager>().shield = true;
        if (!ultOn) {
            moveScript.movementSpeed = shieldMoveSpeed;
        }
        if (ultOn) {
            moveScript.movementSpeed = ultShieldMoveSpeed;
        }
        gO_shield.SetActive(true);
        disabled = true;
        if (playShieldAnimOnce) {
            anim_shieldOpening.Play();
            playShieldAnimOnce = false;
        }
        shieldActive = true;
        if (shieldHealth <= 0) {
            shieldHealth = 0;
            shieldBroken = true;
            AbilityShieldEnd();
        }
    }

    void AbilityShieldEnd() {
        gO_regularCamera.SetActive(true);
        gO_shieldCamera.SetActive(false);
        GetComponent<PlayerHealthManager>().shield = false;
        moveScript.movementSpeed = normalMoveSpeed;
        gO_shield.SetActive(false);
        playShieldAnimOnce = true;
        disabled = false;
        shieldActive = false;
    }

    void ShieldRegen() {
        if (!isShieldRegening && shieldHealth < shieldMaxHealth) {
            shieldRegenTimer += Time.deltaTime;
            if (shieldRegenTimer >= shieldRegenDelay) {
                shieldRegenTimer = 0;
                isShieldRegening = true;
            }
        }
        if (isShieldRegening) {
            shieldTimer += Time.deltaTime;
            while (shieldTimer >= shieldTicker) {
                shieldTimer -= shieldTicker;
                shieldHealth += (shieldRegenPerSec / 20);
            }
            if (shieldHealth >= shieldMaxHealth) {
                shieldHealth = shieldMaxHealth;
                isShieldRegening = false;
            }
        }
    }
    void ShieldBroken() {
        shieldBreakTimer += Time.deltaTime;
        if (shieldBreakTimer >= shieldBreakTicker) {
            shieldBreakTimer -= shieldBreakTicker;
            shieldBreakCounter++;
        }
        if (shieldBreakCounter >= shieldBreakCD) {
            shieldBroken = false;
            shieldBreakCounter = 0;
        }
    }

    //Shield Bash Code
    void AbilityShieldBash() {
        if((Input.GetButton(inputPrefix + "PrimaryFire") && !sbActive) || shieldBashInput > 0.9f && !sbActive) {
            sbActive = true;
        }
    }

    void ShieldBashMove() {
        if (sbActive) {
            sbMoveTimer += Time.deltaTime;
            var thrust = transform.forward * (sbRange / sbMoveDuration);
            rig.velocity = thrust;
            moveScript.enabled = false;
            ShieldBashActive();
            if (sbMoveTimer >= sbMoveDuration) {
                SbEnd();
            }
        }
    }

    void SbEnd() {
        sbMoveTimer = 0;
        sbActive = false;
        sbCooldown = true;
    }
    void ShieldBashActive() {
        Transform shieldPos = gO_shield.transform;
        Collider[] hitList = Physics.OverlapBox(shieldPos.position + shieldPos.forward * 0.25f, new Vector3(1.5f, 2f, 0.25f), shieldPos.rotation, enemyLayer);
        if (hitList.Length > 0) {
            var enemyHit = hitList[0];
            enemyHit.GetComponentInParent<IDamageable>().TakeDamage(sbDamage);
            AddUltCharge(sbDamage);
            enemyHit.GetComponentInParent<IStunable>().Stun(sbStunDuration);
            enemyHit.GetComponentInParent<IStunable>().DamageKnockback(enemyHit.transform.position - transform.position, 3, 1);
            SbEnd();
        }
    }

    void ShieldBashCooldown() {
        moveScript.enabled = true;
        sbCooldownTimer += Time.deltaTime;
        if (sbCooldownTimer >= 1) {
            sbCooldownTimer -= 1;
            sbCooldownCounter++;
        }
        if (sbCooldownCounter >= sbCooldownDuration) {
            sbCooldownCounter = 0;
            sbCooldown = false;
        }
    }

    //Ultimate Code
    void AbilityUlt() {
        if (Input.GetButtonDown(inputPrefix + "AbilityUlt")) {
            ultOn = true;
            ultDisable = true;
        }
    }

    void UltActive() {
        ultReady = false;
        ultTimer += Time.deltaTime;
        Collider[] playersHit = Physics.OverlapSphere(transform.position, ultRadius, playerLayers);
        ultArmorTimer += Time.deltaTime;
        if (ultArmorTimer >= ultArmorTicker) {
            ultArmorTimer -= ultArmorTicker;
            foreach(Collider player in playersHit) {
                if (player.GetComponent<PlayerHealthManager>() != null) {
                    player.GetComponent<PlayerHealthManager>().ReceiveTempArmor(ultTempArmorPerTick, ultTempArmorDuration, ultMaxTempArmor);
                }
                if (player.GetComponent<PlayerMover>() != null) {
                    player.GetComponent<PlayerMover>().movementSpeed = ultMoveSpeed;
                }
            }
        }
        if (ultTimer >= ultDuration) {
            foreach (Collider player in playersHit) {
                player.GetComponent<PlayerMover>().movementSpeed = normalMoveSpeed;
            }
            ultCharge = 0;
            ultOn = false;
        }
    }

    void UltRecovery() {
        ultRecoveryTimer += Time.deltaTime;
        if (ultRecoveryTimer >= ultRecoveryDuration) {
            ultRecoveryTimer = 0;
            ultDisable = false;
        }
    }

    //UI Stuff
    void HealUI() {
        if (healCharges == healMaxCharges) {
            healUI.text = "Heal " + healCharges;
            healUI.color = Color.white;
            healChargeUI.text = "";
        }
        if (healCharges < healMaxCharges) {
            healUI.text = "Heal  " + healCharges;
            healUI.color = Color.yellow;
            healChargeUI.text = "" + chargeRestoreCounter;
        }
        if (healCharges == 0) {
            healUI.color = Color.red;
        }
    }

    void WhipshotUI() {
        if (!whipshotCD) {
            whipshotUI.text = "Whipshot";
            whipshotUI.color = Color.white;
        }
        if (whipshotCD) {
            whipshotUI.text = "" + (whipshotCooldown - whipshotCounter);
            whipshotUI.color = Color.red;
        }
    }

    void ShieldUI() {
        if (!shieldBroken) {
            ui_shield.text = "Shield";
            ui_shield.color = Color.white;
            ui_shieldHPInactive.text = "" + Mathf.RoundToInt(shieldHealth);
            ui_shieldHPInactive.color = ui_shieldOKColor;
        }
        if (shieldBroken) {
            ui_shield.text = "" + (shieldBreakCD - shieldBreakCounter);
            ui_shield.color = Color.red;
            ui_shieldHPInactive.text = "" + shieldHealth;
            ui_shieldHPInactive.color = Color.red;
        }
        if (shieldActive) {
            ui_shieldHPActive.text = "Shield: " + Mathf.RoundToInt(shieldHealth) + " / " + shieldMaxHealth;
        }
        if (!shieldActive) {
            ui_shieldHPActive.text = "";
        }
    }
    void ShieldBashUI() {
        if (sbCooldown || !shieldActive) {
            ui_sbability.color = Color.red;
        }
        if (sbCooldown) {
            ui_sbability.text = "" + (sbCooldownDuration - sbCooldownCounter);
            return;
        }
        if (!sbCooldown) {
            ui_sbability.text = "Bash";
        }
        if (shieldActive) {
            ui_sbability.color = Color.white;
        }
    }

    void UltimateUI() {
        ui_ult.text = "" + Mathf.RoundToInt((ultCharge / maxUltCharge) * 100) + "%";
        ui_ult.color = Color.white;
    }

    void UI() {
        HealUI();
        WhipshotUI();
        ShieldUI();
        ShieldBashUI();
        UltimateUI();
    }

    public void AddUltCharge(float amount) {
        if (ultCharge < maxUltCharge) {
            ultCharge += amount;
            if (ultCharge >= maxUltCharge) {
                ultCharge = maxUltCharge;
            }
        }
    }

}
