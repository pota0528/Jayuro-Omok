using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPanelController : MonoBehaviour
{
    public void OnClickPlayButton()
    {
        UIManager.Instance.ChangeToGameScene();
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
