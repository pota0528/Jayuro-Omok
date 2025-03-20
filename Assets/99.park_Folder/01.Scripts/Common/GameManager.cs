using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace park_namespace
{
    public class GameManager : Singleton<GameManager>
    {
        private Canvas _canvas;
        
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject signUpPanel;
        [SerializeField] private GameObject userPanel;
        [SerializeField] private GameObject profilePanel;
        //추가: 옵션패널 
        
        
        private void Start()
        {
            OpenLoginPanel();
        }

        public void OpenLoginPanel()
        {
            
            if (_canvas != null)
            {
                Debug.Log("로그인패널열기");
              Instantiate(loginPanel, _canvas.transform);
               
            }
        }

        public void OpenSignUpPanel()
        {
          
            if (_canvas != null)
            {
             Instantiate(signUpPanel, _canvas.transform);
               
            }
        }

        public void OpenUserPanel()
        {
            if (_canvas != null)
            {
                Instantiate(userPanel, _canvas.transform);
            }
        }

        public void OpenProfilePanel()
        {
            if (_canvas != null)
            {
                Instantiate(profilePanel, _canvas.transform);
            }
            
        }

   

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _canvas = GameObject.FindObjectOfType<Canvas>();
        }
    }

}
