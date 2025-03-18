using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Joe_namespace
{
    public class MainPanelController : MonoBehaviour
    {
        public void OnClickPlayButton()
        {
            GameManager.Instance.ChangeToGameScene();
        }
        
        
        public void OnClickShopPanelButton()
        {
            GameManager.Instance.OpenShopPanel();
            Debug.Log("Shop Panel Opened");
        }

        public void OnClickRankingPanelButton()
        {
            GameManager.Instance.OpenRankingPanel();
            Debug.Log("Ranking Panel Opened");
        }

      
        
    }

}