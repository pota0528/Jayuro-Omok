using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource BgmAudioSource;
    [SerializeField] private AudioSource SfxAudioSource;
    [SerializeField] private AudioSource SfxSelectorAudioSource;
    public AudioMixer audioMixer;
    public AudioClip[] audioClip;
    
    private void Start()
    {
        // 저장된 볼륨 값을 불러옴
        PlayerPrefs.SetFloat("BGMSlider", 0.2f);//슬라이더바
        PlayerPrefs.SetFloat("SFXSlider", 0.2f);
        PlayerPrefs.Save();
        
        SetBGMVolume(PlayerPrefs.GetFloat("BGMSlider")); // 초기 BGM 볼륨 설정
        SetSFXVolume(PlayerPrefs.GetFloat("SFXSlider")); // 초기 SFX 볼륨 설정
        
        OnPlayBGM(0);
    }
    
    public void OnPutStone() // 바둑알 놓을 때
    {
        SfxAudioSource.Play();
    }

    public void OnPlayBGM(int num) // 배경음
    {
        BgmAudioSource.clip = audioClip[num];
        BgmAudioSource.Play();
    }

    public void OnPutSelector()
    {
        if (!SfxSelectorAudioSource.isPlaying)
        {
            SfxSelectorAudioSource.Play();
        }
    }
    
    public void SetBGMVolume(float sliderVolume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(sliderVolume) * 20);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        audioMixer.SetFloat("SFXSelector", Mathf.Log10(volume) * 20);
        PlayerPrefs.Save();
    }
    
    private void PlaySceneBGM(string sceneName)
    {
        AudioClip newClip = null;

        // 씬 이름에 따라 BGM 변경
        if (sceneName == "Login")
        {
            newClip = audioClip[0];
        }
            
        else if (sceneName == "Game")
        {
            newClip = audioClip[1];
        }
        
        if (newClip != null && BgmAudioSource.clip != newClip)
        {
            BgmAudioSource.clip = newClip;
            BgmAudioSource.loop = true;
            BgmAudioSource.Play();
        }
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneBGM(scene.name);
    }
}