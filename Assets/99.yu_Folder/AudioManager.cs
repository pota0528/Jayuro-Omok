using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace yu_namespace{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource BgmAudioSource;
        [SerializeField] public AudioSource SfxAudioSource;

        public void OnPutStone()//바둑알 놓을때
        {
            SfxAudioSource.Play();
        }

        public void OnPlayBGM()//배경음
        {
            BgmAudioSource.Play();
        }

    }

}
  

