using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace yu_namespace
{
    public class PopupController : BaseUIController
    {
        [SerializeField] private TextMeshProUGUI messageText;

        //메세지 바꾸기
        public void Show(string message)
        {
            base.Show();
            messageText.text = message;
        }
        
        
    }
    
}

