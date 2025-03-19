using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

namespace park_namespace
{
    public class UserSessionManager : MonoBehaviour
    {
        public static UserSessionManager _instance;

        public static UserSessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UserSessionManager>();
                }
                if (_instance == null)
                {
                    Debug.LogError("UserSessionManager가 씬에 존재하지 않습니다.");
                }

                return _instance;
            }
        }
        
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

    
    }
}

