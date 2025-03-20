using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace park_namespace
{
    public class GameManager : Singleton<GameManager>
    {
    #region 찬영 UI 관련 
        private Canvas _canvas;
        
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject signUpPanel;
        [SerializeField] private GameObject userPanel;
        [SerializeField] private GameObject profilePanel;
        //추가: 옵션패널 
        
        //DB관련
        public GameObject playerPrefab;
        private DBManager mongoDBManager;
        
        //프로필 이미지 인덱스를 관리하는 변수 
        private int currentIamgeIndex = 0; 
        
        
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
        //이미지 인덱스 설정
        public void SetProfileImageIndex(int index)
        {
            currentIamgeIndex = index;
        }
        
        //현재 이미지 인덱스를 가져오는 메서드
        public int GetProfileImageIndex()
        {
            return currentIamgeIndex;
        }
        // 이미지 업데이트
        public void UpdateUserProfileImage(Sprite newProfileImage)
        {
            UserPanelController userPanelController = FindObjectOfType<UserPanelController>();
            if (userPanelController != null)
            {
                userPanelController.UpdateProfileImage(newProfileImage);
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
    

    #endregion

}
