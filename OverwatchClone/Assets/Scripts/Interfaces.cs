using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    void TakeDamage(float damage);
}

public interface Iai {
    void Death();
}

public interface IStoppable {
    void StopMovement();
}

public interface IStunable {
    void Stun(float duration);
    void DamageKnockback(Vector3 direction, float magnitude, float verticalLaunch);
}

public interface IUltCharge {
    void AddUltCharge(float amount);
}