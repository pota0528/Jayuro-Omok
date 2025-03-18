using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// using Joe_namespace;

// namespace Joe_namespace
// {
    public class MainPanelController : MonoBehaviour
    {
        public void OnClickShopPanelButton()
        {
            GameManager.Instance.OpenShopPanel();
            Debug.Log("Shop Panel Opened");
        }

        public void OnClickRankingPanelButton()
        {
            GameManager.Instance.OpenRankingPanel();
        }
    }

// }