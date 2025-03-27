using UnityEngine;

[CreateAssetMenu(fileName = "LaserEnemyData", menuName = "ScriptableObjects/Enemies/LaserEnemy", order = 6)]
public class LaserEnemySO : ScriptableObject {
    
    [Header("Laser Enemy Config")]
    [Tooltip("Duration the Laser enemy will be in its weak damage state with damage set to WeakDamagePerSecond.\nAfter this duration, its damage will increase to StrongDamagePerSecond.")]
    public float WeakDamageDuration = 5;
    [Tooltip("Duration the Laser enemy will be stunned for after getting hit by a canon shot. Once the stun is over, it will attack the player with weak damage.")]
    public float StunDuration = 3;
    [Tooltip("Before the end of the stun, the Laser enemy will animate itself rotating towards the player. This value determines the length of the animation, but does not affect the stun duration.\nThis means that if StunDuration = 3 and ReArmDuration = 1, the re-arm phase will start 2 seconds into the stun.")]
    public float ReArmDuration = 1;
    [Tooltip("Weak damage value.")]
    public float WeakDamagePerSecond = 10;
    [Tooltip("Strong damave value.")]
    public float StrongDamagePerSecond = 50;
    [Tooltip("Laser color when damage is weak.")]
    public Gradient WeakColor;
    [Tooltip("Laser colro when damage is strong.")]
    public Gradient StrongColor;
    
}