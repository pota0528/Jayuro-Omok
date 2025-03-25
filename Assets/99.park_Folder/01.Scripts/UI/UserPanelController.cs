using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using park_namespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


    public class UserPanelController : Singleton<UserPanelController>
    {
        [SerializeField] private TMP_Text userNameText;
        [SerializeField] private TMP_Text cointText;
        [SerializeField] private Button profileButton;

        public void Start()
        {
            UpdataUI();
        }
        //PlayerData를 저장 
        private PlayerData playerData;

        public void UpdataUI()
        {
            playerData = UserSessionManager.Instance.GetPlayerData();
            if (playerData != null)
            {
                userNameText.text = playerData.level+"급 "+playerData.nickname;
                cointText.text = "코인: " +playerData.coin.ToString(); 
                //GameManager에서 저장된 이미지 인덱스를 가져와서 프로필 이미지 갱신 
                //저장된 프로필 이미지 인덱스를 적용
                Sprite profileImage =UIManager.Instance.GetProfileImage(playerData.imageIndex);
              profileButton.GetComponent<Image>().sprite=profileImage;

            }
            else
            {
                Debug.Log("사용자 정보가 없습니다.");
            }
            
        }
        //프로필 이미지 갱신 메서드
        public void UpdateProfileImage(Sprite newProfileImage)
        {
            // UserPanel의 프로필 이미지 갱신
            profileButton.GetComponent<Image>().sprite = newProfileImage;
            
        }

    public void OnClickStartButton()
        {
            SceneManager.LoadScene("Game");
            Debug.Log("게임 시작");
        }

        public void OnClickReplayButton()
        {
            SceneManager.LoadScene("ReplayScene");
            Debug.Log("기보 보기");
        }

        public void OnClickRanckingButton()
        {
            UIManager.Instance.OpenRankingPanel();
            Debug.Log("랭킹 보기");
        }

        public void OnClickShopButton()
        {
            UIManager.Instance.OpenShopPanel();
            Debug.Log("상점 보기");
        }

        public void OnClickSettingBUtton()
        {
            UIManager.Instance.OpenSettingPopup();
        }

        public void OnClickProfileButton()
        {
            UIManager.Instance.OpenProfilePanel();
        }
    }


