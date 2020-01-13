using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputPrefixSwitcher : MonoBehaviour
{
    public PlayerMover s76Mover;
    public PlayerMover brigMover;
    public PlayerAbilitiesBrigitte brigAbilities;
    public PlayerAbilitiesSoldier76 s76Abilities;
    public PlayerWeaponMelee s76Melee;
    public PlayerWeaponRanged s76Ranged;
    public PlayerBrigitteMelee brigMelee;
    public MouseLook s76Cam;
    public MouseLook brigCam;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) {
            s76Mover.inputPrefix = "P1";
            s76Abilities.inputPrefix = "P1";
            s76Melee.inputPrefix = "P1";
            s76Ranged.inputPrefix = "P1";
            s76Cam.inputPrefix = "P1";
            s76Cam.isControllableByMouse = true;
            brigMover.inputPrefix = "P2";
            brigAbilities.inputPrefix = "P2";
            brigMelee.inputPrefix = "P2";
            brigCam.inputPrefix = "P2";
            brigCam.isControllableByMouse = false;
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            s76Mover.inputPrefix = "P2";
            s76Abilities.inputPrefix = "P2";
            s76Melee.inputPrefix = "P2";
            s76Ranged.inputPrefix = "P2";
            s76Cam.inputPrefix = "P2";
            s76Cam.isControllableByMouse = false;
            brigMover.inputPrefix = "P1";
            brigAbilities.inputPrefix = "P1";
            brigMelee.inputPrefix = "P1";
            brigCam.inputPrefix = "P1";
            brigCam.isControllableByMouse = true;
        }
    }
}
