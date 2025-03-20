using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Joe_namespace
{
    public class GamePanelController : MonoBehaviour
    {
        public void OnClickMainButton()
        {
            GameManager.Instance.ChangeToMainScene();
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


