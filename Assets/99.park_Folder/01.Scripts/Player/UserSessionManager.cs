using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;


    public class UserSessionManager :Singleton<UserSessionManager>
    {
      
        
        private PlayerData _playerData;

        // 로그인한 유저 정보를 설정
        public void SetPlayerData(PlayerData playerData)
        {
            _playerData = playerData;
        }
        public PlayerData GetPlayerData()
        {
            return _playerData;
        }

       
        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            
        }
    }


