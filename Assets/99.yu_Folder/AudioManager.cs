using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource BgmAudioSource;
    [SerializeField] private AudioSource SfxAudioSource;
    [SerializeField] private AudioSource SfxSelectorAudioSource;
    [SerializeField] private AudioMixer audioMixer;

    public void OnPutStone() // 바둑알 놓을 때
    {
        SfxAudioSource.Play();
    }

    public void OnPlayBGM() // 배경음
    {
        BgmAudioSource.Play();
    }

    public void OnPutSelector()
    {
        if (!SfxSelectorAudioSource.isPlaying)
        {
            Debug.Log("소리재생");
            SfxSelectorAudioSource.Play();
        }
    }

    public void OnPauseBGM()
    {
        BgmAudioSource.Pause();
    }

    private void Start()
    {
        // 저장된 볼륨 값을 불러옴
        float savedBGMVolume = PlayerPrefs.GetFloat("BGMParam", 0.75f);
        SetBGMVolume(savedBGMVolume); // 초기 BGM 볼륨 설정
        
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXParam", 0.75f);
        SetSFXVolume(savedSFXVolume); // 초기 SFX 볼륨 설정
        
        OnPlayBGM();
        Debug.Log(BgmAudioSource.isPlaying);
    }

    public void SetBGMVolume(float volume)
    {
        // BGM 볼륨 설정
        if (volume <= 0)
        {
            volume = 0.0001f;
        }
        audioMixer.SetFloat("BGMParam", Mathf.Log10(volume) * 20);

        // PlayerPrefs에 볼륨 값 저장 (변경 시마다)
        PlayerPrefs.SetFloat("BGMParam", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        // BGM 볼륨 설정
        if (volume <= 0)
        {
            volume = 0.0001f;
        }
        audioMixer.SetFloat("SFXParam", Mathf.Log10(volume) * 20);
        audioMixer.SetFloat("SFXSelectorParam", Mathf.Log10(volume) * 20);

        // PlayerPrefs에 볼륨 값 저장 (변경 시마다)
        PlayerPrefs.SetFloat("SFXParam", volume);
        PlayerPrefs.SetFloat("SFXSelectorParam", volume);
        PlayerPrefs.Save();
    }
    
    public void SetSFXSelectorVolume(float volume)
    {
        // Selector 볼륨 설정
        if (volume <= 0)
        {
            volume = 0.0001f;
        }
        audioMixer.SetFloat("SFXSelectorParam", Mathf.Log10(volume) * 20);

        // PlayerPrefs에 볼륨 값 저장 (변경 시마다)
        PlayerPrefs.SetFloat("SFXSelectorParam", volume);
        PlayerPrefs.Save();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}