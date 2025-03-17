using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

namespace yu_namespace
{
    public class GameManager : MonoBehaviour
    {
        
        [SerializeField] private GameObject messagePopupPrefab;
        [SerializeField] private GameObject settingPopupPrefab;
        [SerializeField] private RectTransform parent;
        
        //메시지팝업 오픈
        public void OpenMessagePopup(string msg)
        {
            var messagePopup = Instantiate(messagePopupPrefab, parent);
            messagePopup.GetComponent<PopupController>().Show(msg);
        }

        //설정팝업 오픈
        public void OpenSettingPopup()
        {
            var settingPopup = Instantiate(settingPopupPrefab, parent);
            settingPopup.GetComponent<BaseUIController>().Show();
        }

        //테스트팝업오픈
        public void TestShowMessagePopupButton()
        {
            OpenMessagePopup("로그인이 되었습니다.");
        }

        public void TestShowSettingPopupButton()
        {
            OpenSettingPopup();
        }

        

    }
}

