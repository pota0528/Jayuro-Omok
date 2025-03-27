using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GamePanelController : MonoBehaviour
{
    public void OnClickMainButton()
    {
        //UIManager.Instance.ChangeToMainScene();
    }
        
    public void OnClickShopPanelButton()
    {
        UIManager.Instance.OpenShopPanel();
        Debug.Log("Shop Panel Opened");
    }

    public void OnClickRankingPanelButton()
    {
        UIManager.Instance.OpenRankingPanel();
        Debug.Log("Ranking Panel Opened");
    }
}

