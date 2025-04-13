using UnityEngine;
using UnityEngine.Audio;

public class AudioPlayer2D : MonoBehaviour {
    
    public static AudioPlayer2D Instance;
    
    public enum EClipSFX {
        Cannon_Kill,
        Cannon_Hit,
        Vacuum_Kill,
        Weapon_CannonShot,
        Plr_OutOfFuel,
        Plr_PickupFuel,
        Plr_RotateWoosh
    }
    
    public enum EClipMusic {
        None,
        Test
    }
    
    [Header("Audio Source and Mixer References")]
    public AudioMixer MainAudioMixer;
    public AudioSource asMusic;
    public AudioSource asGeneralSFX;
    public AudioSource asMotorcycleDriving;
    public AudioSource asMotorcycleRotating;
    public AudioSource asCannon;
    
    [Header("Sound Effect References")]
    [SerializeField]
    AudioClip sfx_Cannon_Kill;
    [SerializeField]
    AudioClip sfx_Cannon_Hit;
    [SerializeField]
    AudioClip sfx_Vacuum_Kill;
    [SerializeField]
    AudioClip sfx_Weapon_CannonShot;
    [SerializeField]
    AudioClip sfx_Plr_OutOfFuel;
    [SerializeField]
    AudioClip sfx_Plr_PickupFuel;
    [SerializeField]
    AudioClip sfx_Plr_RotateWoosh;
    
    [Header("Music references")]
    [SerializeField]
    AudioClip mus_Test;
    
    [Header("Temporary")]
    public float MaxMotorcyclePitch = 1.5f;
    public float MaxMotorcyclePitchSpeed = 45;
    public bool enableRotationSound = true;
    
    // float lastSpeed = 0;
    PlayerCharacterCtrlr plr;
    
    
    
    void Awake() {
        Instance = this;
        GameManager.A_PlayerSpawned += OnPlayerSpawned;
        GameManager.A_PlayerDestroying += OnPlayerDestroying;
        // TODO: Create multiple sfx players to play sounds from
    }

    void Update() {
        if (!plr) return;
        float speed = plr.rb.velocity.magnitude;
        asMotorcycleDriving.pitch = Mathf.Lerp(asMotorcycleDriving.pitch, Mathf.Lerp(1, MaxMotorcyclePitch, speed / MaxMotorcyclePitchSpeed), 0.3f);
        // lastSpeed = speed;
    }

    public void PlayClipSFX(EClipSFX clip) {
        switch (clip) {
        case EClipSFX.Cannon_Kill:
            asGeneralSFX.PlayOneShot(sfx_Cannon_Kill);
            break;
        case EClipSFX.Cannon_Hit:
            asGeneralSFX.PlayOneShot(sfx_Cannon_Hit);
            break;
        case EClipSFX.Vacuum_Kill:
            asGeneralSFX.PlayOneShot(sfx_Vacuum_Kill);
            break;
        case EClipSFX.Weapon_CannonShot:
            asCannon.Play();
            break;
        case EClipSFX.Plr_OutOfFuel:
            // asSFX.PlayOneShot(sfx_Plr_OutOfFuel);
            asGeneralSFX.clip = sfx_Plr_OutOfFuel;
            asGeneralSFX.Play();
            break;
        case EClipSFX.Plr_PickupFuel:
            // asSFX.clip = sfx_Plr_PickupFuel;
            // asSFX.Play();
            asGeneralSFX.PlayOneShot(sfx_Plr_PickupFuel);
            break;
        case EClipSFX.Plr_RotateWoosh:
            if (!enableRotationSound) return;
            asMotorcycleRotating.Play();
            break;
        }
    }
    
    public void PlayClipMusic(EClipMusic clip) {
        switch (clip) {
        case EClipMusic.None:
            asMusic.Stop();
            break;
        case EClipMusic.Test:
            asMusic.clip = mus_Test;
            asMusic.Play();
            break;
        }
    }
    
    void OnPlayerSpawned(PlayerCharacterCtrlr p) {
        asMotorcycleDriving.Play();
        plr = p;
    }
    
    void OnPlayerDestroying(PlayerCharacterCtrlr p) {
        asMotorcycleDriving.Stop();
        plr = null;
    }
    
    public void SetMasterVolume(float volume) {
        MainAudioMixer.SetFloat("volMaster", calcLogarithmicVolume(volume));
    }
    
    public void SetSFXVolume(float volume) {
        MainAudioMixer.SetFloat("volSFX", calcLogarithmicVolume(volume));
    }
    
    public void SetMusicVolume(float volume) {
        MainAudioMixer.SetFloat("volMusic", calcLogarithmicVolume(volume));
    }
    
    float calcLogarithmicVolume(float volume) { // linear int (or float) to log
        volume /= 100f;
        return volume <= 0.01f ? -80 : 20 * Mathf.Log10(volume);
    }
    
}