
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
        private PlayerData playeData;
        
        //프로필 이미지 인덱스를 관리하는 변수 
        private int currentIamgeIndex = 0; 
        
        

        
        private void Start()
        {
           OpenLoginPanel();
           OpenStartTitlePanel();
          
            //playeData=UserSessionManager.Instance.GetPlayerData();
            mongoDBManager = FindObjectOfType<DBManager>();
           
        }
//로그인 후 Player데이터 설정 
        public void SetPlayerData(PlayerData playerData)
        {
            this.playeData = playerData;
            Debug.Log(playerData.nickname);
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
            var (playerData,message) = mongoDBManager.Login(id, password);

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
    
    

    #endregion
    
    
    #region 장운 씬이동,패널연결
    [Header("장운")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject rankingPanel;
    [HideInInspector] public int coinCount;
    
    protected override void Awake()
    {
        base.Awake();
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

    [SerializeField] private RectTransform parent;
        
    //스타트 타이틀 패널
    public void OpenStartTitlePanel()
    {
        
        var startTitlePanel = Instantiate(startTitlePanelPrefab, parent);
    }
        //메시지팝업 오픈
        public void OpenMessagePopup(string msg)
        {
            
            var messagePopup = Instantiate(messagePopupPrefab, parent);
            messagePopup.GetComponent<MessagePopupController>().Show(msg);
            
        }

        //설정팝업 오픈
        public void OpenSettingPopup()
        {
            var settingPopup = Instantiate(settingPopupPrefab, parent);
            settingPopup.GetComponent<BaseUIController>().Show();
        }
        
        //옵션 패널 오픈
        public void OpenGiveupPanel()
        {
            var giveupPanel = Instantiate(giveupPanelPrefab, parent);
            giveupPanel.GetComponent<BaseUIController>().Show();

        }
        
        //노코인 패널 오픈
        public void OpenNoCoinPanel()
        {
            var noCoinPanel = Instantiate(noCoinPanelPrefab, parent);
            noCoinPanel.GetComponent<BaseUIController>().Show();
            noCoinPanel.GetComponent<NoCoinController>().ShowCoinText(playeData.coin);//찬영님이 주시는 데이터 형태로
        }

        public void OpenWinLosePanel()//GameResult형의 gameResult (GameResult gameResult)
        {
            //todo: EndGame(GameResult gameResult) 메소드 일부 넣기 밑에 주석 코드 두줄 주석 해제하면 됨.
            //_gameUIController.SetGameUIMode(GameUIController.GameUIMode.GameOver);
            //_blockGontroller.OnBlockClickedDelegate=null;
            
            var winLosePanel = Instantiate(winLosePanelPrefab, parent);
            winLosePanel.GetComponent<WinLosePanelController>().ShowCoinText(playeData.coin);
            int currentLevelCount = winLosePanel.GetComponent<WinLosePanelController>().GetLevelCount(playeData.level);//안에 들어가는 수는 level
            bool resultPanel = winLosePanel.GetComponent<WinLosePanelController>().SetResultPanel(currentLevelCount, YuConstants.isWin);
                
            if (resultPanel == false)//승급,강등 = false | 게이지 바 패널 = true
            {
                var ResultPanel= Instantiate(messagePopupPrefab, parent);
                if (playeData.levelPoint > 0)
                {
                    ResultPanel.GetComponent<MessagePopupController>().Show("승급하셨습니다.\n급수 : "+ playeData.level);
                    playeData.levelPoint = 0;
                }
                else if (playeData.levelPoint < 0)
                {
                    ResultPanel.GetComponent <MessagePopupController>().Show("강등되셨습니다.\n급수 : "+playeData.level);
                    playeData.levelPoint = 0;
                }
                
            }
            
            if (YuConstants.isWin==false)
            {
                var mainWinLosePanel = Instantiate(mainWinLosePanelPrefab, parent);
                mainWinLosePanel.GetComponent<MainWinLosePanelController>().MainLosePanelOpen();
            }
            else
            {
                var mainWinLosePanel = Instantiate(mainWinLosePanelPrefab, parent);
                mainWinLosePanel.GetComponent<MainWinLosePanelController>().MainWinPanelOpen();
            }

            

        }
        

    #endregion
    

}