using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class AudioPlayer2D : MonoBehaviour {
    
    public enum EClipSFX {
        Kill_DirectHit,
        Weapon_CanonShot,
        Plr_OutOfFuel,
        Plr_PickupFuel
    }
    
    public enum EClipMusic {
        None
    }
    
    [Header("Audio Source References")]
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
    
    public void PlayClipSFX(EClipSFX clip) {
        switch (clip) {
        case EClipSFX.Kill_DirectHit:
            asSFX.clip = sfx_Kill_DirectHit;
            asSFX.Play();
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
        }
    }
    
    public void PlayClipMusic(EClipMusic clip) {
        switch (clip) {
        case EClipMusic.None:
            asMusic.Stop();
            break;
        }
    }
    
}