using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPopupController : BaseUIController
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    public static Action<bool> OnMute;
    private void Start()
    {
        //슬라이더 초기화
        bgmSlider.value = PlayerPrefs.GetFloat("BGMSlider");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXSlider");

        bgmSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        
    }
    
    private void OnEnable()
    {
        OnMute += UpdateSliders;
    }

    private void OnDisable()
    {
        OnMute -= UpdateSliders;
    }

    private void UpdateSliders(bool isMuted)
    {
        bgmSlider.value = isMuted ? bgmSlider.minValue : PlayerPrefs.GetFloat("BGMSlider");
    }

    private void OnDestroy()
    {
        // 슬라이더 값 변경 시 PlayerPrefs에 저장
        PlayerPrefs.SetFloat("BGMSlider", bgmSlider.value);
        PlayerPrefs.SetFloat("SFXSlider", sfxSlider.value);
        PlayerPrefs.Save();
    }
}