using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerAbilitiesSoldier76 : MonoBehaviour, IUltCharge {
    public string inputPrefix = "P1";
    public PlayerIdentifier playerIdentifier;

    public float sprintSpeedMultiplier = 1.5f;
    float runSpeed;
    float baseSpeed;
    float sprintTimer = 0;
    public bool canRun = true;
    public bool isRunning = false;
    bool endingRun = false;
    PlayerMover moveScript;
    PlayerWeaponRanged gunScript;
    public GameObject healAbilityPrefab;
    bool ability3CooldownOn = false;
    float ability3CooldownTimer = 0;
    float ability3CooldownTicker = 1f;
    public int healCooldown = 15;
    int healCooldownCounter = 0;
    public Text healUI;
    public GameObject rocketAbilityPrefab;
    float rocketInput = 0f;
    public GameObject cameraLook;
    bool ability1CooldownOn = false;
    float ability1CooldownTimer = 0;
    float ability1CooldownTicker = 1f;
    public int rocketCooldown = 8;
    int rocketCooldownCounter = 0;
    public Text rocketUI;
    public float ultCharge = 0;
    public float ultChargeMax = 2310f;
    float ultTimer = 0f;
    float ultTicker = 1f;
    bool ultReady = false;
    bool ultOn = false;
    public Text ultUI;
    float normalGunDeviation;
    float normalGunReload;
    public float ultiGunReload = 0.65f;
    public int ultDuration = 6;
    public float ultAngle = 15;
    public float ultMaxDistance = 50;
    float ultTimer2 = 0;
    float ultTicker2 = 1;
    int ultCounter = 0;
    public LayerMask enemyLayer;
    public Vector3 newGunRay;
    public LayerMask groundMask;
    List<Collider> seenEnemies = new List<Collider>();
    public Camera playerCamera;
    public GameObject ultPotentialMarker;
    List<GameObject> ultPotentialMarkersInactive = new List<GameObject>();
    List<GameObject> ultPotentialMarkersActive = new List<GameObject>();
    public Collider ultActiveTarget;
    public GameObject ultActiveTargetMarker;
    public Transform canvas;

    void Start() {
        ultCharge = 0;
        playerIdentifier = gameObject.GetComponent<PlayerIdentifier>();
        moveScript = gameObject.GetComponent<PlayerMover>();
        gunScript = gameObject.GetComponent<PlayerWeaponRanged>();
        normalGunDeviation = gunScript.maxDeviation;
        normalGunReload = gunScript.reloadTime;
        runSpeed = moveScript.movementSpeed * sprintSpeedMultiplier;
        baseSpeed = moveScript.movementSpeed;
        for (int i = 0; i < 30; i++) {
            var marker = Instantiate(ultPotentialMarker, canvas);
            ultPotentialMarkersInactive.Add(marker);
            marker.SetActive(false);
        }
    }

    private void Update() {
        if (canRun) {
            SAbilitySprint();
        }
        if (isRunning && !canRun) {
            SprintEndBuffer();
        }
        if (ability3CooldownOn) {
            HealCooldown();
            healUI.text = "Heal " + healCooldownCounter;
            healUI.color = Color.red;
        }
        if (!ability3CooldownOn) {
            healUI.text = "Heal";
            healUI.color = Color.white;
            healCooldownCounter = healCooldown;
            SAbilityHeal();
        }
        if (ability1CooldownOn) {
            RocketCooldown();
            rocketUI.text = "Rocket " + rocketCooldownCounter;
            rocketUI.color = Color.red;
        }
        if (!ability1CooldownOn) {
            rocketUI.text = "Rocket";
            rocketUI.color = Color.white;
            rocketCooldownCounter = rocketCooldown;
            SAbilityRockets();
        }
        if (!ultReady && !ultOn) {
            ultTimer += Time.deltaTime;
            if (ultTimer >= ultTicker) {
                ultTimer -= ultTicker;
                ultCharge += 5;
            }
            if (ultCharge >= ultChargeMax) {
                ultReady = true;
            }
            ultUI.text = "" + (Mathf.RoundToInt((ultCharge / ultChargeMax * 100)));
            ultUI.color = Color.white;
        }
        if (ultReady && !ultOn) {
            SAbilityUlti();
            ultUI.text = "Ultimate";
            ultUI.color = Color.blue;
        }
        if (ultOn) {
            UltiActive();
        }
        rocketInput = Input.GetAxis(inputPrefix + "Ability1");

    }
    public void SAbilitySprint() {
        if (Input.GetButton(inputPrefix + "Ability2") && canRun) {
            SprintStart();
        }
        if (isRunning && (Input.GetButton(inputPrefix + "PrimaryFire") || gunScript.shoot > 0.9f)) {
            SprintEndBuffer();
            endingRun = false;
        }
        if (isRunning && Input.GetButtonDown(inputPrefix + "Reload") && gunScript.ammo < gunScript.maxAmmo) {
            gunScript.disabled = false;
            gunScript.isReloading = true;
            SprintEnd();
        }
        if (isRunning && !Input.GetButton(inputPrefix + "Ability2")) {
            SprintEndBuffer();
            endingRun = true;
        }
    }

    public void SAbilityHeal() {
        if (Input.GetButton(inputPrefix + "Ability3")) {
            Instantiate(healAbilityPrefab, transform.position, new Quaternion(0, 0, 0, 0));
            ability3CooldownOn = true;
        }
    }

    public void SAbilityRockets() {
        if (rocketInput > 0.9f) {
            Instantiate(rocketAbilityPrefab, gunScript.gunOffsetPoint.position, cameraLook.transform.rotation);
            ability1CooldownOn = true;
        }
    }
    public void SAbilityUlti() {
        if (Input.GetButton(inputPrefix + "AbilityUlt")) {
            UltiStart();
        }
    }

    void SprintStart() {
        if (endingRun) {
            moveScript.moverSprint(runSpeed);
            endingRun = false;
        }
        if (isRunning) {
            Sprint();
            return;
        }
        gunScript.disabled = true;
        isRunning = true;
    }

    void Sprint() {
        moveScript.moverSprint(runSpeed);
    }
    void SprintEndBuffer() {
        if (endingRun && Input.GetButton(inputPrefix + "Ability2")) {
            SprintStart();
            return;
        }
        moveScript.moverSprint(baseSpeed);
        sprintTimer += Time.deltaTime;
        float sprintTicker = 0.3f;
        if (sprintTimer >= sprintTicker) {
            sprintTimer = 0;
            SprintEnd();
        }
        
    }
    void SprintEnd() {
        gunScript.disabled = false;
        moveScript.moverSprint(baseSpeed);
        isRunning = false;
    }

    void HealCooldown() {
        ability3CooldownTimer += Time.deltaTime;
        if (ability3CooldownTimer >= ability3CooldownTicker) {
            ability3CooldownTimer -= ability3CooldownTicker;
            healCooldownCounter--;
        }
        if (healCooldownCounter <= 0 && ability3CooldownOn) {
            ability3CooldownOn = false;
        }
    }
    void RocketCooldown() {
        ability1CooldownTimer += Time.deltaTime;
        if (ability1CooldownTimer >= ability1CooldownTicker) {
            ability1CooldownTimer -= ability1CooldownTicker;
            rocketCooldownCounter--;
        }
        if (rocketCooldownCounter <= 0 && ability1CooldownOn) {
            ability1CooldownOn = false;
        }
    }

    void UltiStart() {
        gunScript.ammo = gunScript.maxAmmo;
        gunScript.maxDeviation = 0;
        gunScript.reloadTime = ultiGunReload;
        ultOn = true;
        gunScript.ultOn = true;
        gunScript.canHeadshot = false;
    }

    void UltiActive() {
        UltiSeekTargets();
        ultTimer2 += Time.deltaTime;
        if (ultTimer2 >= ultTicker2) {
            ultTimer2 -= ultTicker2;
            ultCounter++;
        }
        if (ultCounter >= ultDuration) {
            ultCharge = 0;
            ultOn = false;
            ultReady = false;
            gunScript.reloadTime = normalGunReload;
            gunScript.maxDeviation = normalGunDeviation;
            ultActiveTargetMarker.SetActive(false);
            gunScript.ultOn = false;
            gunScript.canHeadshot = true;
        }
    }
    void UltiSeekTargets() {
        float width = Mathf.Tan(Mathf.Deg2Rad * (ultAngle / 2)) * ultMaxDistance;
        Vector3 centerPoint = gunScript.gunOffsetPoint.position + gunScript.gunOffsetPoint.forward * (ultMaxDistance / 2);
        DrawUltBorders(centerPoint);
        Collider[] enemiesHit = Physics.OverlapBox(centerPoint, new Vector3(width, width, ultMaxDistance/2), gunScript.gunOffsetPoint.rotation, enemyLayer);
        var visible = new List<Collider>();
        var invisible = new List<Collider>();

        foreach (Collider obj in enemiesHit) {
            Vector3 hitDirection = obj.transform.position - gunScript.gunOffsetPoint.transform.position;
            Ray ray = new Ray(gunScript.gunOffsetPoint.position, hitDirection);

            if (!Physics.Raycast(ray, Vector3.Distance(transform.position, obj.transform.position), groundMask) && obj.GetComponent<EnemyColliderLocator>().isBody) {
                if (!visible.Contains(obj)) {
                    visible.Add(obj);
                }
            }
            if (Vector3.Angle(playerCamera.transform.forward, obj.transform.position - transform.position) < ultAngle && obj.GetComponent<EnemyColliderLocator>().isBody) {
                if (!visible.Contains(obj)) {
                    visible.Add(obj);
                }
            }
        }
        foreach (Collider obj in visible) {
            Vector3 hitDirection = obj.transform.position - gunScript.gunOffsetPoint.transform.position;
            Ray ray = new Ray(gunScript.gunOffsetPoint.position, hitDirection);

            if (Physics.Raycast(ray, Vector3.Distance(transform.position, obj.transform.position), groundMask) && obj.GetComponent<EnemyColliderLocator>().isBody) {
                if (!invisible.Contains(obj) && visible.Contains(obj)) {
                    invisible.Add(obj);
                }
            }
            if (Vector3.Angle(playerCamera.transform.forward, obj.transform.position - transform.position) > ultAngle && obj.GetComponent<EnemyColliderLocator>().isBody) {
                if (!invisible.Contains(obj) && visible.Contains(obj)) {
                    invisible.Add(obj);
                }
            }
        }
        foreach (Collider obj in invisible) {
            if (visible.Contains(obj)) {
                visible.Remove(obj);
            }
        }
        print("Hit: " + enemiesHit.Length + " Invisible: " + invisible.Count + " Visible: " + visible.Count);
        if (visible.Count > 0) {
            //for (int i = 0; i < visible.Count; i++) {
            //    ultPotentialMarkersActive.Add(ultPotentialMarkersInactive[i]);
            //    if (ultPotentialMarkersInactive.Contains(ultPotentialMarkersActive[i])) {
            //        ultPotentialMarkersInactive.Remove(ultPotentialMarkersActive[i]);
            //    }
            //    var markerPos = playerCamera.WorldToScreenPoint(visible[i].transform.position);
            //    ultPotentialMarkersActive[i].transform.position = markerPos;
            //    ultPotentialMarkersActive[i].SetActive(true);
            //}
            gunScript.ultOn = true;
            ultActiveTarget = UltFindActiveTarget(visible);
            newGunRay = ultActiveTarget.transform.position - transform.position;
            var markerPos = playerCamera.WorldToScreenPoint(ultActiveTarget.transform.position);
            ultActiveTargetMarker.transform.position = markerPos;
            ultActiveTargetMarker.SetActive(true);

        } else {
            ultActiveTarget = null;
            ultActiveTargetMarker.SetActive(false);
        }
        if (ultActiveTarget == null) {
            gunScript.ultOn = false;
        }
        //var min = invisible.Min(c => Vector3.Angle(playerCamera.transform.forward, c.transform.position - transform.position));
    }

    Collider UltFindActiveTarget(List<Collider> list) {
        var minAngleIndex = 0;
        if (list.Count > 1) {
            for (int i = 0; i < list.Count - 1; i++) {
                var minAngleA = Vector3.Angle(playerCamera.transform.forward, list[i + 1].transform.position - transform.position);
                var minAngleB = Vector3.Angle(playerCamera.transform.forward, list[minAngleIndex].transform.position - transform.position);
                if (minAngleA < minAngleB) {
                    minAngleIndex = i + 1;
                }
            }
            return list[minAngleIndex];
        } else return list[0];
    }

    void DrawUltBorders(Vector3 centerPoint) {
        Debug.DrawRay(gunScript.gunOffsetPoint.position, gunScript.gunOffsetPoint.forward * ultMaxDistance);
        Debug.DrawRay(gunScript.gunOffsetPoint.position, Quaternion.AngleAxis(ultAngle / 2, Vector3.up) * gunScript.gunOffsetPoint.forward * ultMaxDistance);
        Debug.DrawRay(gunScript.gunOffsetPoint.position, Quaternion.AngleAxis(-ultAngle / 2, Vector3.up) * gunScript.gunOffsetPoint.forward * ultMaxDistance);
        Debug.DrawRay(gunScript.gunOffsetPoint.position, Quaternion.AngleAxis(ultAngle / 2, Vector3.right) * gunScript.gunOffsetPoint.forward * ultMaxDistance);
        Debug.DrawRay(gunScript.gunOffsetPoint.position, Quaternion.AngleAxis(-ultAngle / 2, Vector3.right) * gunScript.gunOffsetPoint.forward * ultMaxDistance);
    }

    public void AddUltCharge(float amount) {
        if (ultCharge < ultChargeMax) {
            ultCharge += amount;
            if (ultCharge >= ultChargeMax) {
                ultCharge = ultChargeMax;
            }
        }
    }

    //bool HealCooldownTracker()
    //{
    //    if (healCooldownCounter <= 0 && ability2CooldownOn)
    //    {
    //        ability2CooldownOn = false;
    //        return true;
    //    }
    //    else return false;
    //}
}
