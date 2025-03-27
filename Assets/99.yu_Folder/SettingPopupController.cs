using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingPopupController : BaseUIController
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    
    private void Start()
    {
        //슬라이더 초기화
        float savedBGMVolume = PlayerPrefs.GetFloat("BGMParam", 0.75f);
        bgmSlider.value = savedBGMVolume;
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXParam", 0.75f);
        sfxSlider.value = savedSFXVolume;

        // 슬라이더 값 변경 시 볼륨 업데이트
        bgmSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        

        // 초기 볼륨 설정
        AudioManager.Instance.SetBGMVolume(bgmSlider.value);
        AudioManager.Instance.SetSFXVolume(sfxSlider.value);
        
    }
    
    

    private void OnDestroy()
    {
        // 슬라이더 값 변경 시 PlayerPrefs에 저장
        PlayerPrefs.SetFloat("BGMParam", bgmSlider.value);
        PlayerPrefs.SetFloat("SFXParam", sfxSlider.value);
        PlayerPrefs.Save();
    }
}