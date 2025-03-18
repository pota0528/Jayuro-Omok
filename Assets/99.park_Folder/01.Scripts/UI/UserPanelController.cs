using System.Collections;
using System.Collections.Generic;
using park_namespace;
using UnityEngine;

namespace park_namespace
{
    public class UserPanelController : MonoBehaviour
    {

        public void OnClickStartButton()
        {
            Debug.Log("게임 시작");
        }

        public void OnClickReplayButton()
        {
            Debug.Log("기보 보기");
        }

        public void OnClickRanckingButton()
        {
            Debug.Log("랭킹 보기");
        }

        public void OnClickShopButton()
        {
            Debug.Log("상점 보기");
        }

        public void OnClickSettingBUtton()
        {
            Debug.Log("설정");
        }

        public void OnClickProfileButton()
        {
            GameManager.Instance.OpenProfilePanel();
        }
    }

}
