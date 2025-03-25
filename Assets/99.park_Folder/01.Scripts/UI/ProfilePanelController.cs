using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

    public class ProfilePanelController : PanelController
    {
        //TODO: 버튼 클릭하면 그 이미지 정보를 저장하기 
        [SerializeField] private Button[] profileButtons; //버튼
        

        private void Start()
        {
            SetButtonImage();
            for (int i = 0; i < profileButtons.Length; i++)
            {
                int index = i;
                profileButtons[i].onClick.AddListener(() => OnProfileButtonClicked(index));
            }
        }
        
        // 버튼의 이미지를 배열에 있는 이미지로 적용
        private void SetButtonImage()
        {
            for (int i = 0; i < profileButtons.Length; i++)
            {
                Sprite profileImage= UIManager.Instance.GetProfileImage(i);
                profileButtons[i].GetComponent<Image>().sprite =profileImage;
            }
        }

        //버튼 클릭 시 프로필 이미지 변경 
        public void OnProfileButtonClicked(int index)
        {
            if (index >= 0)
            { 
                //선택한 인덱스를 UIManager에 저장
                UIManager.Instance.SetProfileImageIndex(index);
                //이미지 인덱스를 GameManager에 저장
                 Sprite selectedSprite =UIManager.Instance.GetProfileImage(index);
                 UIManager.Instance.UpdateUserProfileImage(selectedSprite);
                //UiserSessionManager에 프로필 인덱스를 저장
                var playerData = UserSessionManager.Instance.GetPlayerData();
                if (playerData != null)
                {
                    playerData.imageIndex = index;
                    DBManager.Instance.UpdatePlayerData(playerData,updateImageOnly:true);
                }
            }
        }
   

        
        public void OnClickConfirmButton()
        {
            Debug.Log("확인");
            Hide();
        }
   
        public void OnClickBackButton()
        {
            Hide();
        }
    }


