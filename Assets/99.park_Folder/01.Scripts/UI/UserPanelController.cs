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
        
        [SerializeField] private TMP_Text levelPointText;
        
        public static int levelPointCount=0;

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
                cointText.text = "코인: " + playerData.coin.ToString(); 
                levelPointText.text = "승점 포인트: "+playerData.levelPoint;//자현추가
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
            if (playerData.coin < 100)
            {
                UIManager.Instance.OpenNoCoinPanel();
                Debug.Log("노코인패널");
            }
            else
            {
                playerData.coin -= 100;
                SceneManager.LoadScene("Game");
                Debug.Log("게임 시작");
            }
            
            //자현추가
            var winLosePanelObject = UIManager.Instance.winLosePanelPrefab;
            levelPointCount = winLosePanelObject.GetComponent<WinLosePanelController>().GetLevelCount(playerData.level);
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
        
        public string GetPlayerNickname()
        {
            return playerData?.nickname ?? "Unknown"; // playerData가 null일 경우 기본값으로 반환
        }
    }


