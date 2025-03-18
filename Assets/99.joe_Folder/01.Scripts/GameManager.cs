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
        
        private Canvas _canvas;
        
        
        public void OpenShopPanel()
        {
          
            _canvas = FindObjectOfType<Canvas>();
            
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
            }
        }
    
    
        
        
        
    
        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
        }
    
    }
}





