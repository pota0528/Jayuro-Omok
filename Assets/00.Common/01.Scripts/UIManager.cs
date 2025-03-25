
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
        
        //실제 생성된 패널을 추적할 수 있도록 인스턴스 변수를 추가 
        private GameObject loginPanelInstance;
        private GameObject userPanelInstance;
        private GameObject startTitlePanelInstance;
        private GameObject signUpPanelInstance;
        
        private bool isStartTitlePanelActive = false;
        private bool isUserPanelActive=false;
        
        
        //DB관련
        public GameObject playerPrefab;
        private DBManager mongoDBManager;
        private PlayerData playerData;
        
        //프로필 이미지 인덱스를 관리하는 변수 
        private int currentIamgeIndex = 0; 
        
        
        
        private void Start()
        {
            _canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
            if (_canvas == null)
            {
                Debug.Log("캔버스 없음 ");
            }
            
            // 패널 생성 및 초기화
            loginPanelInstance = Instantiate(loginPanel, _canvas.transform);
            userPanelInstance = Instantiate(userPanel, _canvas.transform);
            startTitlePanelInstance =Instantiate(startTitlePanelPrefab, _canvas.transform);
            signUpPanelInstance = Instantiate(signUpPanel, _canvas.transform);
            
            //처음에 비활성화
            loginPanelInstance.SetActive(false);
            userPanelInstance.SetActive(false);
            startTitlePanelInstance.SetActive(false);
            signUpPanelInstance.SetActive(false);
            

            //초기 패널 열기
            OpenLoginPanel();
            OpenStartTitlePanel2();
           mongoDBManager = FindObjectOfType<DBManager>();
           
           // 씬 로드 이벤트 등록
           SceneManager.sceneLoaded += OnSceneLoaded;
           
        }
//로그인 후 Player데이터 설정 
        public void SetPlayerData(PlayerData playerData)
        {
            this.playerData = playerData;
        }

        public void OpenLoginPanel()
        {
            if (_canvas != null && loginPanelInstance != null)
            {
                // 씬 로드 이벤트 등록
                loginPanelInstance.SetActive(true);
                userPanelInstance.SetActive(false); // 로그인 시 UserPanel 비활성화
                startTitlePanelInstance.SetActive(false);
                Debug.Log("로그인패널열기");
            }
        }

        public void OpenSignUpPanel()
        {
          
            if (_canvas != null)
            {
             signUpPanelInstance.SetActive(true);
               
            }
        }

        public void OpenUserPanel()
        {
            if (_canvas != null && userPanelInstance != null)
            {
                loginPanelInstance.SetActive(false); // 로그인 패널 닫기
                userPanelInstance.SetActive(true);
              //  startTitlePanelInstance.SetActive(true); // StartTitle도 함께 표시
                isUserPanelActive = true;
                isStartTitlePanelActive = true;
                Debug.Log("유저 패널 열기");
            }
        }

        public void OpenProfilePanel()
        {
            if (_canvas != null)
            {
                Instantiate(profilePanel, _canvas.transform);
            }
            
        }
        public void OpenStartTitlePanel2()
        {
            if (_canvas != null && startTitlePanelInstance != null)
            {
                startTitlePanelInstance.SetActive(true);
                isStartTitlePanelActive = true; // 상태 저장
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
        

        


        //현재 활성화된 Panel저장
        private void SavePanelState()
        {
            if (_canvas != null)
            {
                isStartTitlePanelActive = startTitlePanelInstance != null && startTitlePanelInstance.activeSelf;
                isUserPanelActive = userPanelInstance != null && userPanelInstance.activeSelf;
            }
        }
        //저장된 Panel복원
        private void RestorePanelState()
        {   
            if (_canvas == null)
            {
                _canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
                if (_canvas == null) return;
            }

            // 패널을 새 캔버스에 붙이기
            loginPanelInstance.transform.SetParent(_canvas.transform, false);
            userPanelInstance.transform.SetParent(_canvas.transform, false);
            startTitlePanelInstance.transform.SetParent(_canvas.transform, false);

            // 상태 복원
            loginPanelInstance.SetActive(false); // 기본적으로 비활성화
            userPanelInstance.SetActive(isUserPanelActive);
            startTitlePanelInstance.SetActive(isStartTitlePanelActive);
        }
        
        //Replay씬으로 이동할때 상태
        public void ChangeToReplayScene()
        {
           SavePanelState();    
            SceneManager.LoadScene("ReplayScene");
        }
        //Game씬으로 이동
        public void ChangeToGameScene2()
        {
            SavePanelState();
            SceneManager.LoadScene("Game");
        }
        
        //Login 씬으로 돌아올때 패널 상태 복원
        public void ChangeToLoginScene()
        {
            SavePanelState();
            SceneManager.LoadScene("Login");
        }
        
        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Login")
            {
                RestorePanelState();
            }
            
        }
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // 메모리 누수 방지
        }
    

    #endregion
    
    
    #region 장운 씬이동,패널연결
    [Header("장운")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject rankingPanel;
    [HideInInspector] public int coinCount;
    
    protected override void Awake()
    {
        base.Awake();
        
        DontDestroyOnLoad(this.gameObject);
        coinCount = UserInformations.CoinCount;
    
            
    }
    
    
    public void ChangeToGameScene()
    {
        SceneManager.LoadScene("Game_jk");
    }
        
    public void ChangeToMainScene()
    {
        SceneManager.LoadScene("Main_jk");
    }
        
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
    [SerializeField] private GameObject winLosePanelPrefab;
    [SerializeField] private GameObject startTitlePanelPrefab;
    [SerializeField] private GameObject mainWinLosePanelPrefab;
        
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

        public void OpenWinLosePanel()//GameResult형의 gameResult (GameResult gameResult)
        {
            //todo: EndGame(GameResult gameResult) 메소드 일부 넣기 밑에 주석 코드 두줄 주석 해제하면 됨.
            //_gameUIController.SetGameUIMode(GameUIController.GameUIMode.GameOver);
            //_blockGontroller.OnBlockClickedDelegate=null;
            
            var winLosePanel = Instantiate(winLosePanelPrefab, _canvas.transform);
            winLosePanel.GetComponent<WinLosePanelController>().ShowCoinText(playerData.coin);
            int currentLevelCount = winLosePanel.GetComponent<WinLosePanelController>().GetLevelCount(playerData.level);//안에 들어가는 수는 level
            bool resultPanel = winLosePanel.GetComponent<WinLosePanelController>().SetResultPanel(currentLevelCount, YuConstants.isWin);
                
            if (resultPanel == false)//승급,강등 = false | 게이지 바 패널 = true
            {
                var ResultPanel= Instantiate(messagePopupPrefab, _canvas.transform);
                if (playerData.levelPoint > 0)
                {
                    ResultPanel.GetComponent<MessagePopupController>().Show("승급하셨습니다.\n급수 : "+ playerData.level);
                    playerData.levelPoint = 0;
                }
                else if (playerData.levelPoint < 0)
                {
                    ResultPanel.GetComponent <MessagePopupController>().Show("강등되셨습니다.\n급수 : "+playerData.level);
                    playerData.levelPoint = 0;
                }
                
            }
            
            if (YuConstants.isWin==false)
            {
                var mainWinLosePanel = Instantiate(mainWinLosePanelPrefab, _canvas.transform);
                mainWinLosePanel.GetComponent<MainWinLosePanelController>().MainLosePanelOpen();
            }
            else
            {
                var mainWinLosePanel = Instantiate(mainWinLosePanelPrefab, _canvas.transform);
                mainWinLosePanel.GetComponent<MainWinLosePanelController>().MainWinPanelOpen();
            }
            
        }
        

    #endregion
    

}