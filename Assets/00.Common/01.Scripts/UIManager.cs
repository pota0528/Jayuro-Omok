
using System;
using UnityEngine;

using UnityEngine.SceneManagement;

    public class UIManager : Singleton<UIManager>
    {
        
    #region 찬영 UI 관련 
        private Canvas _canvas;
        [Header("찬영")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject signUpPanel;
        [SerializeField] private GameObject userPanel;
        [SerializeField] private GameObject profilePanel;
        [SerializeField] private Sprite[] profileSprites;
         
        //추가: 옵션패널 
        
        //DB관련
        public GameObject playerPrefab;
        private DBManager mongoDBManager;
        private PlayerData playerData;
        
        //프로필 이미지 인덱스를 관리하는 변수 
        private int currentIamgeIndex = 0; 
        
        //UserPanel에 대한 참조 추가
        private GameObject userPanelInstance;

        private bool isUserPanelActive = false;
        private void Start()
        {
            _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
           OpenLoginPanel();
           OpenStartTitlePanel();
           
           if (_canvas != null && userPanel != null)
           {
               userPanelInstance = Instantiate(userPanel, _canvas.transform);
               userPanelInstance.SetActive(false); // Initially inactive
           }
          
            //playeData=UserSessionManager.Instance.GetPlayerData();
            mongoDBManager = FindObjectOfType<DBManager>();
           
        }
//로그인 후 Player데이터 설정 
        public void SetPlayerData(PlayerData playerData)
        {
            this.playerData = playerData;
            Debug.Log(playerData.nickname);
        }

        public void OpenLoginPanel()
        {
            if (_canvas != null)
            {
                // Deactivate the user panel if it's active
                if (userPanelInstance != null)
                {
                    userPanelInstance.SetActive(false);
                    isUserPanelActive = false; // Track that the user panel was deactivated
                }

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
            SignUpPanelController signUpPanelController = FindObjectOfType<SignUpPanelController>();
            if (userPanelController != null)
            {
                userPanelController.UpdateProfileImage(newProfileImage);
            }

            if (signUpPanelController != null)
            {
                signUpPanelController.UpdateProfileImage(newProfileImage);
            }
           
        }

        public Sprite GetProfileImage(int index)
        {
            if (index >= 0 && index < profileSprites.Length)
            {
                return profileSprites[index];
            }
            else
            {
                Debug.Log("유효하지않은");
                return null;
            }
        }


        public void LoginPlayer(string id, string password)
        {
            var (playerData, message) = mongoDBManager.Login(id, password);

            if (playerData != null)
            {
                // 플레이어 프리팹이 없으므로 직접 플레이어 객체를 생성합니다.
                GameObject playerObject = new GameObject("Player");  // 새로운 플레이어 오브젝트 생성
                PlayerManager playerScript = playerObject.AddComponent<PlayerManager>();  // PlayerManager 컴포넌트 추가

                if (playerScript != null)
                {
                    playerScript.SetPlayerData(playerData);  // 로그인된 플레이어 데이터 설정
                }

                // 로그인 성공 시 UserPanel을 활성화
                if (userPanelInstance != null)
                {
                    userPanelInstance.SetActive(true);
                    Debug.Log("UserPanel 활성화됨");
                }
                else
                {
                    Debug.LogError("UserPanel 인스턴스가 없습니다.");
                }
            }
            else
            {
                Debug.LogError("로그인 실패: " + message);
            }
        }



        
        
        

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _canvas = GameObject.FindObjectOfType<Canvas>();
            if (_canvas == null)
            {
                Debug.LogError("Canvas 객체를 찾을 수 없습니다.");
            }
            else
            {
                Debug.Log(_canvas);
            }
            
            if (scene.name == "Login")
            {
                
                OpenUserPanel();
                OpenStartTitlePanel();
            }
        }
    
    

    #endregion
    
    
    #region 장운 씬이동,패널연결
    [Header("장운")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private ShopConfirmPopup confirmPopupPrefab;
    [HideInInspector] public int coinCount;
    
    private UserPanelController userPanelController;

    
    /// <summary>
    /// 확인 팝업 호출 (외부에서 호출 가능)
    /// </summary>
    public void ShowConfirmPopup(string message, Action onConfirm)
    {
        if (_canvas != null && confirmPopupPrefab != null)
        {
            var popup = Instantiate(confirmPopupPrefab, _canvas.transform);
            popup.Show(message, onConfirm);
        }
        else
        {
            Debug.LogWarning("ConfirmPopup 프리팹이 연결되지 않았거나 캔버스 없음.");
        }
    }
    public void UpdateCoin(int amount)
    {
        if (playerData != null)
        {
            playerData.coin += amount;
            DBManager.Instance.UpdatePlayerData(playerData);
            UserSessionManager.Instance.SetPlayerData(playerData);
            SetPlayerData(playerData);

            if (SceneManager.GetActiveScene().name == "Login")
            {
                UserPanelController.Instance.UpdataUI();
            }
        }
    }
    // public void ChangeToGameScene()
    // {
    //     SceneManager.LoadScene("Game_jk");
    // }
    //     
    // public void ChangeToMainScene()
    // {
    //     SceneManager.LoadScene("Main_jk");
    // }
        
    public void OpenShopPanel()
    {
        if (_canvas != null)
        {
            var shopPanelObject = Instantiate(shopPanel, _canvas.transform);
            shopPanelObject.GetComponent<PanelController>().Show();
            Debug.Log("상점 떳드아!");
        }
    }

    public void OpenRankingPanel()
    {
        if (_canvas != null)
        {
            var rankingPanelObject = Instantiate(rankingPanel, _canvas.transform);
            rankingPanelObject.GetComponent<PanelController>().Show();
            Debug.Log("랭킹 떳드아!!!");
        }
    }
    #endregion

    #region 자현 UI 관련
  [Header("자현")]
    [SerializeField] private GameObject messagePopupPrefab;
    [SerializeField] private GameObject settingPopupPrefab;
    [SerializeField] private GameObject giveupPanelPrefab;
    [SerializeField] private GameObject noCoinPanelPrefab;
    public GameObject winLosePanelPrefab;
    [SerializeField] private GameObject startTitlePanelPrefab;
    [SerializeField] private GameObject mainWinLosePanelPrefab;
    [SerializeField] private GameObject UpDownResultPanelPrefab;

    public GameObject winLosePanelObject;
        
    //스타트 타이틀 패널
    public void OpenStartTitlePanel()
    {
        var startTitlePanel = Instantiate(startTitlePanelPrefab, _canvas.transform);
    }
        //메시지팝업 오픈
        public void OpenMessagePopup(string msg)
        {
            
            var messagePopup = Instantiate(messagePopupPrefab, _canvas.transform);
            messagePopup.GetComponent<MessagePopupController>().Show(msg);
            
        }

        //설정팝업 오픈
        public void OpenSettingPopup()
        {
            var settingPopup = Instantiate(settingPopupPrefab, _canvas.transform);
            settingPopup.GetComponent<BaseUIController>().Show();
        }
        
        //옵션 패널 오픈
        public void OpenGiveupPanel()
        {
            var giveupPanel = Instantiate(giveupPanelPrefab, _canvas.transform);
            giveupPanel.GetComponent<BaseUIController>().Show();

        }
        
        //노코인 패널 오픈
        public void OpenNoCoinPanel()
        {
            var noCoinPanel = Instantiate(noCoinPanelPrefab, _canvas.transform);
            noCoinPanel.GetComponent<BaseUIController>().Show();
            noCoinPanel.GetComponent<NoCoinController>().ShowCoinText(playerData.coin);//찬영님이 주시는 데이터 형태로
        }

        public void OpenWinLosePanel(GameManager.GameResult gameResult)//GameResult형의 gameResult (GameResult gameResult)
        { 
            var winLosePanel = Instantiate(winLosePanelPrefab, _canvas.transform);
            winLosePanel.GetComponent<WinLosePanelController>().ShowCoinText(playerData.coin);
            winLosePanel.GetComponent<WinLosePanelController>().ShowResultPanel();
            int currentLevelCount = winLosePanel.GetComponent<WinLosePanelController>().GetLevelCount(playerData.level);//안에 들어가는 수는 level
            bool resultPanel = winLosePanel.GetComponent<WinLosePanelController>().SetResultPanel(currentLevelCount, gameResult);
                
            if (resultPanel == false)//승급,강등 = false | 게이지 바 패널 = true
            {
                var ResultPanel= Instantiate(UpDownResultPanelPrefab, _canvas.transform);
                ResultPanel.GetComponent<MessagePopupController>().ShowResultPanel();
                
                if (playerData.levelPoint > 0)
                {
                    ResultPanel.GetComponent<MessagePopupController>().Show("승급하셨습니다.\n급수 : "+ playerData.level);
                    playerData.levelPoint = 0;
                    SetPlayerData(playerData);
                }
                else if (playerData.levelPoint < 0)
                {
                    ResultPanel.GetComponent <MessagePopupController>().Show("강등되셨습니다.\n급수 : "+playerData.level);
                    playerData.levelPoint = 0;
                    SetPlayerData(playerData);
                }
                
            }
            
            if (gameResult==GameManager.GameResult.Lose)
            {
                var mainWinLosePanel = Instantiate(mainWinLosePanelPrefab, _canvas.transform);
                mainWinLosePanel.GetComponent<MainWinLosePanelController>().MainLosePanelOpen();
            }
            else
            {
                var mainWinLosePanel = Instantiate(mainWinLosePanelPrefab, _canvas.transform);
                mainWinLosePanel.GetComponent<MainWinLosePanelController>().MainWinPanelOpen();
            }
            
            winLosePanelObject = winLosePanel;
        }
        

    #endregion
    

}