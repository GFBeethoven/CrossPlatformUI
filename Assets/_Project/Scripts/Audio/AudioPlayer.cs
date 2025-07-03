using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance { get; private set; }

    [SerializeField] private UIAudioClip _ui;
    [SerializeField] private SfxAudioClip _sfx;
    [SerializeField] private AudioClip _titleScreenPassClip;

    [SerializeField] private AudioSource _uiSource;
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    public void Initialize()
    {
        Instance = this;
    }

    public void PlayButtonClick() => PlayUIShot(_ui.ButtonClick);
    public void PlaySelect() => PlayUIShot(_ui.Select);
    public void PlayToggle() => PlayUIShot(_ui.Toggle);
    public void PlaySlider() => PlayUIShot(_ui.Slider);
    public void PlayPrevTab() => PlayUIShot(_ui.PrevTab);
    public void PlayNextTab() => PlayUIShot(_ui.NextTab);
    public void PlayNavigationBack() => PlayUIShot(_ui.Back);

    public void PlayGunShot() => PlaySfxShot(_sfx.GunShot);
    public void PlayTargetHit() => PlaySfxShot(_sfx.TargetHit);

    public void PlayTitleScreenPass() => PlaySfxShot(_titleScreenPassClip);

    private void PlayUIShot(AudioClip clip)
    {
        _uiSource.PlayOneShot(clip);
    }
    
    private void PlaySfxShot(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    [Serializable]
    public class UIAudioClip
    {
        public AudioClip ButtonClick;

        public AudioClip Select;

        public AudioClip Toggle;

        public AudioClip Slider;

        public AudioClip Back;

        public AudioClip NextTab;

        public AudioClip PrevTab;
    }

    [Serializable]
    public class SfxAudioClip
    {
        public AudioClip GunShot;

        public AudioClip TargetHit;
    }
}
