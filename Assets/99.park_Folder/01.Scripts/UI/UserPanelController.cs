using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using park_namespace;
using TMPro;
using UnityEngine;

namespace park_namespace
{
    public class UserPanelController : MonoBehaviour
    {
        [SerializeField] private TMP_Text userNameText;
        [SerializeField] private TMP_Text cointText;

        public void Start()
        {
            UpdataUI();
        }
        //PlayerData를 저장 
        private PlayerData playerData;

        public void UpdataUI()
        {
            PlayerData playerData = UserSessionManager.Instance.GetPlayerData();
            if (playerData != null)
            {
                userNameText.text = playerData.level+"급 "+playerData.nickname;
                cointText.text = "코인: " +playerData.coin.ToString(); 
            }
            else
            {
                Debug.Log("사용자 정보가 없습니다.");
            }
            
        }

    public void OnClickStartButton()
        {
            Debug.Log("게임 시작");
        }

        public void OnClickReplayButton()
        {
            Debug.Log("기보 보기");
        }

        public void OnClickRanckingButton()
        {
            Debug.Log("랭킹 보기");
        }

        public void OnClickShopButton()
        {
            Debug.Log("상점 보기");
        }

        public void OnClickSettingBUtton()
        {
            Debug.Log("설정");
        }

        public void OnClickProfileButton()
        {
            GameManager.Instance.OpenProfilePanel();
        }
    }

}
