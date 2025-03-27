using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource BgmAudioSource;
    [SerializeField] private AudioSource SfxAudioSource;
    [SerializeField] private AudioSource SfxSelectorAudioSource;
    [SerializeField] private AudioMixer audioMixer;
    public AudioClip[] audioClip;

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

    private void Start()
    {
        // 저장된 볼륨 값을 불러옴
        float savedBGMVolume = PlayerPrefs.GetFloat("BGMParam", 0.75f);
        SetBGMVolume(savedBGMVolume); // 초기 BGM 볼륨 설정
        
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXParam", 0.75f);
        SetSFXVolume(savedSFXVolume); // 초기 SFX 볼륨 설정
        
        OnPlayBGM(0);
    }

    public void SetBGMVolume(float volume)
    {
        // BGM 볼륨 설정
        if (volume <= 0)
        {
            volume = 0.01f;
            
        }
        
        

        audioMixer.SetFloat("BGMParam", Mathf.Log10(volume) * 20);

    }

    public void SetSFXVolume(float volume)
    {
        if (volume <= 0)
        {
            
            volume = 0.01f;
        }
        
        audioMixer.SetFloat("SFXParam", Mathf.Log10(volume) * 20);
        audioMixer.SetFloat("SFXSelectorParam", Mathf.Log10(volume) * 20);
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