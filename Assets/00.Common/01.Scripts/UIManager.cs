
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
        
        

        
        private void Start()
        {
            _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
           OpenLoginPanel();
           OpenStartTitlePanel();
          
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
                Debug.Log("로그인패널열기");
              Instantiate(loginPanel, _canvas.transform);//수정
               
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
            if (_canvas == null)
            {
                Debug.LogError("Canvas 객체를 찾을 수 없습니다.");
            }
            else
            {
                Debug.Log(_canvas);
            }
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
    [SerializeField] private GameObject UpDownResultPanelPrefab;
        
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
            // GameManager.Instance._gameUIController.SetGameUIMode(GameUIController.GameUIMode.GameOver);
            // GameManager.Instance._blockController.OnBlockClickedDelegate=null;
            
            var winLosePanel = Instantiate(winLosePanelPrefab, _canvas.transform);
            winLosePanel.GetComponent<WinLosePanelController>().ShowCoinText(playerData.coin);
            int currentLevelCount = winLosePanel.GetComponent<WinLosePanelController>().GetLevelCount(playerData.level);//안에 들어가는 수는 level
            bool resultPanel = winLosePanel.GetComponent<WinLosePanelController>().SetResultPanel(currentLevelCount, gameResult);
                
            if (resultPanel == false)//승급,강등 = false | 게이지 바 패널 = true
            {
                var ResultPanel= Instantiate(UpDownResultPanelPrefab, _canvas.transform);
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
            
            
            
        }
        

    #endregion
    

}