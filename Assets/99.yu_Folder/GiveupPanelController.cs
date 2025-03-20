using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using yu_namespace;

namespace yu_namespace
{
    
    public class GiveupPanelController : BaseUIController
    {
        public void OnClickGiveupButton()
        {
            Hide(() =>
            {
                //TODO: 메인씬+프로필패널로 이동
                //SceneManager.LoadScene("");
                Debug.Log("메인씬으로 이동");
                //todo: 승점포인트 -1처리하기
                YuConstants.levelPoint--;
                
                

            });
        }
    }
}


