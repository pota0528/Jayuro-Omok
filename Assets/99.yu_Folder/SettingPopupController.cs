using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using yu_namespace;

namespace yu_namespace
{
    public class SettingPopupController : BaseUIController
    {
        public AudioMixer audioMixer;
        public Slider bgmSlider;
        public Slider sfxSlider;

        public void SetBGMVolume()
        {
            audioMixer.SetFloat("BGMParam", Mathf.Log10(bgmSlider.value) * 20);
        }

        public void SetSFXVolume()
        {
            audioMixer.SetFloat("SFXParam", Mathf.Log10(sfxSlider.value) * 20);
        }


    }
    
}  

