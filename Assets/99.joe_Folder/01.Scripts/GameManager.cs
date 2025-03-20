using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Joe_namespace
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private GameObject rankingPanel;

        [HideInInspector] public int coinCount;
        
        
        private Canvas _canvas;


        protected override void Awake()
        {
            base.Awake();
            coinCount = UserInformations.CoinCount;
            
        }
        
        private void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");
            UserInformations.CoinCount = coinCount;
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
                shopPanelObject.GetComponent<PanelController>().ShowPanel();
                Debug.Log("상점 떳드아!");
            }
        }

        public void OpenRankingPanel()
        {
            if (_canvas != null)
            {
                var rankingPanelObject = Instantiate(rankingPanel, _canvas.transform);
                rankingPanelObject.GetComponent<PanelController>().ShowPanel();
                Debug.Log("랭킹 떳드아!!!");
            }
        }
    
        
    
        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {

            //DontDestroyOnLoad(gameObject);

            _canvas = GameObject.FindObjectOfType<Canvas>();
        }
    
    }
}





