using UnityEngine;
using UnityEngine.Audio;

public class AudioPlayer2D : MonoBehaviour {
    
    public static AudioPlayer2D Instance;
    
    public enum EClipSFX {
        Kill_DirectHit,
        Weapon_CanonShot,
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
    public AudioSource asSFX;
    public AudioSource asMusic;
    
    [Header("Sound Effect References")]
    [SerializeField]
    AudioClip sfx_Kill_DirectHit;
    [SerializeField]
    AudioClip sfx_Weapon_CanonShot;
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
    public bool enableRotationSound = true;
    
    
    
    void Awake() {
        Instance = this;
        // TODO: Create multiple sfx players to play sounds from
    }
    
    public void PlayClipSFX(EClipSFX clip) {
        switch (clip) {
        case EClipSFX.Kill_DirectHit:
            asSFX.PlayOneShot(sfx_Kill_DirectHit);
            break;
        case EClipSFX.Weapon_CanonShot:
            asSFX.clip = sfx_Weapon_CanonShot;
            asSFX.Play();
            break;
        case EClipSFX.Plr_OutOfFuel:
            // asSFX.PlayOneShot(sfx_Plr_OutOfFuel);
            asSFX.clip = sfx_Plr_OutOfFuel;
            asSFX.Play();
            break;
        case EClipSFX.Plr_PickupFuel:
            // asSFX.clip = sfx_Plr_PickupFuel;
            // asSFX.Play();
            asSFX.PlayOneShot(sfx_Plr_PickupFuel);
            break;
        case EClipSFX.Plr_RotateWoosh:
            if (!enableRotationSound) return;
            asSFX.clip = sfx_Plr_RotateWoosh;
            asSFX.Play();
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
    
    public void SetMasterVolume(float volume) {
        MainAudioMixer.SetFloat("volMaster", calcLogarithmicVolume(volume));
    }
    
    public void SetSFXVolume(float volume) {
        MainAudioMixer.SetFloat("volSFX", calcLogarithmicVolume(volume));
    }
    
    public void SetMusicVolume(float volume) {
        MainAudioMixer.SetFloat("volMusic", calcLogarithmicVolume(volume));
    }
    
    float calcLogarithmicVolume(float volume) {
        volume /= 100f;
        return volume <= 0.01f ? -80 : 20 * Mathf.Log10(volume);
    }
    
}