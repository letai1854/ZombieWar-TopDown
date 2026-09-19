using UnityEngine;

public static class AnimData
{
    // --- COMMON / BASE PARAMETERS ---
    public static readonly int SpeedHash = Animator.StringToHash("Speed");
    public static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    public static readonly int AttackHash = Animator.StringToHash("Attack");

    // --- PLAYER SPECIFIC ---
    public static readonly int RifleIdleHash = Animator.StringToHash("Rifle_Idle");
    public static readonly int RifleShootHash = Animator.StringToHash("Rifle_Shoot");
    public static readonly int IsShootingHash = Animator.StringToHash("IsShooting");

    // --- ENEMY SPECIFIC ---
    public static readonly int EnemyHitHash = Animator.StringToHash("Hit");
}