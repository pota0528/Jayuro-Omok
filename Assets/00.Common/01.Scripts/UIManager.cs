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
        private bool isLogin = false; //로그인 상태 추적 
        private GameObject loginPanelInstance;
        private void Start()
        {
            Debug.Log("isLoig:"+isLogin);
            InitializeUI();
           
            if(isLogin==true)
            {
                // If already logged in, open the user panel
                OpenUserPanel();
                Debug.Log("유저패널 호출 ");
            }
        }

        private void InitializeUI()
        {
            _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            if (_canvas == null)
            {
                Debug.LogError("Canvas 객체를 찾을 수 없습니다.");
                return;
            }

            mongoDBManager = FindObjectOfType<DBManager>();
            if (_canvas != null && userPanel != null)
            {
                if (userPanelInstance == null || !userPanelInstance)
                {
                    userPanelInstance = Instantiate(userPanel, _canvas.transform);
                }
                userPanelInstance.SetActive(false); // 처음에는 비활성화
            }

            if (isLogin && playerData != null)
            {
                Debug.Log("로그인 되어있음, 플레이어데이터이씅");
                OpenUserPanel();
            }
            else
            {
                Debug.Log("로그인 해야함 ");
                OpenLoginPanel();
            }
            OpenStartTitlePanel();
        }

//로그인 후 Player데이터 설정 
        public void SetPlayerData(PlayerData playerData)
        {
            this.playerData = playerData;
            Debug.Log(playerData.nickname);
            OpenUserPanel(); 
        }

        public void OpenLoginPanel() 
        {
            Debug.Log("로그인 패널 열림");

            // Canvas가 존재하는지 확인
            if (_canvas == null)
            {
                Debug.LogError("Canvas가 없습니다.");
                return;
            }

            // loginPanel이 null인지 확인하고, null일 경우 에러 처리
            if (loginPanel == null)
            {
                Debug.LogError("Login Panel이 할당되지 않았습니다.");
                return;
            }

            // loginPanel이 이미 인스턴스화 되어 있는지 확인
            if (loginPanelInstance == null)
            {
                // 로그인 패널을 _canvas 하위에 인스턴스화
                loginPanelInstance = Instantiate(loginPanel, _canvas.transform);
                loginPanelInstance.SetActive(true); // 로그인 패널을 활성화
                Debug.Log("Login Panel 인스턴스화 및 활성화됨");
            }
            else if (!loginPanelInstance.activeSelf)
            {
                // 이미 존재하는 loginPanelInstance가 비활성화되었을 경우 활성화
                loginPanelInstance.SetActive(true);
                Debug.Log("이미 존재하는 Login Panel을 활성화함");
            }

            // User Panel이 활성화된 상태라면 비활성화
            if (userPanelInstance != null && userPanelInstance.activeSelf)
            {
                userPanelInstance.SetActive(false);
                Debug.Log("User Panel 비활성화됨");
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
            if (_canvas == null)
            {
                Debug.LogError("Canvas가 없습니다.");
                return;
            }

            if (userPanel == null)
            {
                Debug.LogError("User Panel이 할당되지 않았습니다.");
                return;
            }

            // userPanel이 이미 활성화되어 있지 않은 경우
            if (userPanelInstance == null || !userPanelInstance.activeSelf)
            {
                if (userPanelInstance == null)
                {
                    // userPanel이 null일 경우 새로 인스턴스화
                    userPanelInstance = Instantiate(userPanel, _canvas.transform);
                }
                userPanelInstance.SetActive(true);
                Debug.Log("User Panel 활성화됨");
            }
            else
            {
                Debug.Log("User Panel이 이미 활성화되어 있습니다.");
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
                SetPlayerData(playerData);
                isLogin = true;
                
                GameObject playerObject = new GameObject("Player");
                PlayerManager playerScript = playerObject.AddComponent<PlayerManager>();
                if (playerScript != null)
                {
                    playerScript.SetPlayerData(playerData);
                }
                if (loginPanel != null)
                {
                    loginPanel.SetActive(false); // 로그인 패널 숨기기
                }
                OpenUserPanel();
            }
            else
            {
                Debug.LogError("로그인 실패: " + message);
            }
        }

        
        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _canvas = FindObjectOfType<Canvas>();
            if (_canvas == null)
            {
                Debug.LogError("Canvas 객체를 찾을 수 없습니다.");
                return;
            }
            if (scene.name == "Login")
            {
                //씬 로드 시 UI 재초기화
                InitializeUI();
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

    public GameObject NoCoinNextWinLosePanel;
    public int currentLevelCountData;
        
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
            
            NoCoinNextWinLosePanel = winLosePanel;
            currentLevelCountData = currentLevelCount;
        }
        

    #endregion
    

}