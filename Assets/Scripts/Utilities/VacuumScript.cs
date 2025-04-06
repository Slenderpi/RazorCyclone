using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VacuumScript : MonoBehaviour {
    
    [SerializeField]
    AudioSource VacuumAudio;
    [SerializeField]
    PlayerCharacterCtrlr pchar;
    [SerializeField]
    TriggerNotifier SuckboxNotifier;
    [SerializeField]
    Transform VacuumKillbox;
    [SerializeField]
    TriggerNotifier KillboxNotifier;
    [SerializeField]
    GameObject SuckVFX;
    ParticleSystem[] vacVFX;
    
    bool vacuumOn = false;
    float lastVacTogTime = -1000;
    float VacuumAudioLerpTime = 0.1f;
    
    List<Suckable> suckablesInRange;
    readonly int defaultListSize = 7;
    
    void Awake() {
        suckablesInRange = new List<Suckable>(defaultListSize);
        vacVFX = SuckVFX.GetComponentsInChildren<ParticleSystem>();
        DisableVacuum();
        VacuumAudio.volume = 0;
        SuckboxNotifier.A_TriggerEntered += onSuckboxEnter;
        SuckboxNotifier.A_TriggerExited += onSuckboxExit;
        KillboxNotifier.A_TriggerEntered += onKillboxEnter;
    }

    void Start() {
    }

    void FixedUpdate() {
        float alpha = (Time.fixedTime - lastVacTogTime) / VacuumAudioLerpTime;
        VacuumAudio.volume = Mathf.Lerp(0, 1, vacuumOn ? alpha : 1 - alpha);
        if (!vacuumOn) return;
        for (int i = suckablesInRange.Count - 1; i >= 0; i--) {
            Suckable s = suckablesInRange.ElementAt(i);
            if (!s) {
                suckablesInRange.RemoveAt(i);
                continue;
            }
            // TODO: Check that the object is in conical (or frustrum?) range and not behind a wall
            if (s.CanGetVacuumSucked)
                s.rb.AddForce((VacuumKillbox.position - s.transform.position).normalized * pchar.VacuumSuckForce);
        }
    }
    
    public void EnableVacuum() {
        if (!VacuumAudio.isPlaying)
            VacuumAudio.Play();
        vacuumOn = true;
        lastVacTogTime = Time.fixedTime;
        SuckboxNotifier.gameObject.SetActive(true);
        KillboxNotifier.gameObject.SetActive(true);
        SetVFXEnabled(true);
    }
    
    public void DisableVacuum() {
        vacuumOn = false;
        lastVacTogTime = Time.fixedTime;
        suckablesInRange.Clear();
        SuckboxNotifier.gameObject.SetActive(false);
        KillboxNotifier.gameObject.SetActive(false);
        SetVFXEnabled(false);
    }
    
    public void SetVFXEnabled(bool newEnabled) {
        if (newEnabled) {
            foreach (ParticleSystem p in vacVFX) {
                p.Play();
            }
        } else {
            foreach (ParticleSystem p in vacVFX) {
                p.Stop();
            }
        }
    }
    
    void onSuckboxEnter(Collider collider) {
        if (!collider.TryGetComponent(out Suckable s))
            s = collider.GetComponentInParent<Suckable>();
        if (s)
            suckablesInRange.Add(s);
    }
    
    void onSuckboxExit(Collider collider) {
        if (!collider.TryGetComponent(out Suckable s))
            s = collider.GetComponentInParent<Suckable>();
        if (s)
            suckablesInRange.Remove(s);
    }
    
    void onKillboxEnter(Collider collider) {
        if (collider.CompareTag("Enemy")) {
            if (!collider.transform.parent.parent.TryGetComponent(out EnemyBase en)) {
                en = collider.GetComponentInParent<EnemyBase>();
            }
            if (en != null && !en.Dead)
                en.TakeDamage(en.MaxHealth, EDamageType.Vacuum);
        }
    }
    
    // void OnEnable() {
    //     SuckboxNotifier.A_TriggerEntered += onSuckboxEnter;
    //     SuckboxNotifier.A_TriggerExited += onSuckboxExit;
    //     KillboxNotifier.A_TriggerEntered += onKillboxEnter;
    // }
    
    // void OnDisable() {
    //     suckablesInRange.Clear();
    //     SuckboxNotifier.A_TriggerEntered -= onSuckboxEnter;
    //     SuckboxNotifier.A_TriggerExited -= onSuckboxExit;
    //     KillboxNotifier.A_TriggerEntered -= onKillboxEnter;
    // }
    
}