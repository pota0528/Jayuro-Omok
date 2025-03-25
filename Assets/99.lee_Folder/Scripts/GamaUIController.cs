using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
    {
        [SerializeField] private Image playerAImage;
        [SerializeField] private Image playerBImage;
        [SerializeField] private Button gameOverButton;
        [SerializeField] private TextMeshProUGUI userNicknameWithRank;
        [SerializeField] private Image userProfile;
        [SerializeField] private Sprite[] spriteList;

        public enum GameUIMode
        {
            Init,
            TurnA,
            TurnB,
            GameOver
        }

        private const float DisableAlpha = 0.5f;
        private const float EnableAlpha = 1f;
        private int selectedRow = -1;
        private int selectedCol = -1;

        public void DisplayUserInfo(string nickname, string rank, int imageIndex)
        {
            string infoStr = rank + "급 " + nickname;
            userNicknameWithRank.text = infoStr;
            userProfile.sprite = spriteList[imageIndex];
        }
        
        public void SetGameUIMode(GameUIMode mode)
        {
            switch (mode)
            {
                case GameUIMode.Init:
                    playerAImage.color = new Color32(221, 221, 221, 255);
                    playerBImage.color = new Color32(221, 221, 221, 255);
                    break;
                case GameUIMode.TurnA:
                    playerAImage.color = new Color32(105, 255, 132, 255);
                    playerBImage.color = new Color32(221, 221, 221, 255);
                    break;
                case GameUIMode.TurnB:
                    playerAImage.color = new Color32(221, 221, 221, 255);
                    playerBImage.color = new Color32(105, 255, 132, 255);
                    break;
                case GameUIMode.GameOver:
                    //playerAImage[0].gameObject.SetActive(false);
                    //playerBImage[0].gameObject.SetActive(false);
                    //playerAImage[1].gameObject.SetActive(false);
                    //playerBImage[1].gameObject.SetActive(false);
                    break;
            }
        }
        
        /// <summary>
        /// 선택된 위치 저장
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void UpdateSelectedPosition(int row, int col)
        {
            selectedRow = row;
            selectedCol = col;
        }


        /// <summary>
        /// 선택된 위치를 반환하는 메서드 (착수 버튼에서 사용)
        /// </summary>
        /// <returns></returns>
        public (int row, int col) GetSelectedPosition()
        {
            return (selectedRow, selectedCol);
        }
        
        // public void OnClickGameOverButton()
        // {
        //     GameManager.Instance.OpenConfirmPanel("게임을 종료하시겠습니까?", () =>
        //     {
        //         GameManager.Instance.ChangeToMainScene();
        //     });
        // }

        // public void OnClickSettingsButton() 
        // {
        //     GameManager.Instance.OpenSettingsPanel();
        // }

        // public void OnClickBackButton()
        // {
        //     GameManager.Instance.OpenConfirmPanel("게임을 종료하시겠습니까?", () =>
        //     {
        //         GameManager.Instance.ChangeToMainScene();
        //     });
        // }
    }