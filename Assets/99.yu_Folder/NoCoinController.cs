using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yu_namespace;

namespace yu_namespace
{
    public class NoCoinController : BaseUIController
    {
        public void OnClickShopButton()
        {
            Hide(() =>
            {
                //TODO: 메인씬+상점패널로 이동
                //SceneManager.LoadScene("");
                Debug.Log("상점으로 이동");
                
            });
        }
    
        public void OnClickExitButton()
        {
            Hide(() =>
            {
                //TODO: 메인씬+프로필패널로 이동
                //SceneManager.LoadScene("");
                Debug.Log("프로필로 이동");
                
            });
        }
    }
}


