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
        
        //DB관련
        public GameObject playerPrefab;
        private DBManager mongoDBManager;
        private void Start()
        {
            OpenLoginPanel();
            mongoDBManager = FindObjectOfType<DBManager>();
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


        public void LoginPlayer(string id, string password)
        {
            PlayerData playerData = mongoDBManager.Login(id, password);

            if (playerData != null)
            {
                GameObject playerObject = Instantiate (playerPrefab,Vector3.zero,Quaternion.identity);
                PlayerManager playerScript = playerObject.GetComponent<PlayerManager>();

                if (playerObject != null)
                {
                    playerScript.SetPlayerData(playerData);
                }
            }
        }

   

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _canvas = GameObject.FindObjectOfType<Canvas>();
        }
    }

}
