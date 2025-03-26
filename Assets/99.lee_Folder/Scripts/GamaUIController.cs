using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
    {
        [SerializeField] private Image userPlayerTurnPoint;
        [SerializeField] private Image playerAITurnPoint;
        [SerializeField] private Button gameOverButton;
        [SerializeField] private TextMeshProUGUI userNicknameWithRank;
        [SerializeField] private TextMeshProUGUI aiNicknameWithRank;
        [SerializeField] private Image userProfile;
        [SerializeField] private Image aiProfile;
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

        private int rankNumber = 0;

        private string[] aiNickNameList = new string[]
        {
            "예대전설 미드저니",
            "영상장인 소라",
            "잼민이",
            "테무에서 온 알파고",
            "지옥의 코파일럿",
            "트위터키배 그록3",
            "불타는 코파일럿",
        };
        
        public void DisplayUserInfo(string nickname, string rank, int imageIndex)
        {
            rankNumber = int.Parse(rank);
            string infoStr = rank + "급 " + nickname;
            userNicknameWithRank.text = infoStr;
            userProfile.sprite = spriteList[imageIndex];
        }

        public void DisplayAIInfo()
        {
            int randomNumber = Random.Range(0, 8);
            aiNicknameWithRank.text = rankNumber + "급 " + aiNickNameList[randomNumber];
            aiProfile.sprite = spriteList[randomNumber];
        }
        
        public void SetGameUIMode(GameUIMode mode)
        {
            switch (mode)
            {
                case GameUIMode.Init:
                    userPlayerTurnPoint.color = new Color32(221, 221, 221, 255);
                    playerAITurnPoint.color = new Color32(221, 221, 221, 255);
                    break;
                case GameUIMode.TurnA:
                    userPlayerTurnPoint.color = new Color32(105, 255, 132, 255);
                    playerAITurnPoint.color = new Color32(221, 221, 221, 255);
                    break;
                case GameUIMode.TurnB:
                    userPlayerTurnPoint.color = new Color32(221, 221, 221, 255);
                    playerAITurnPoint.color = new Color32(105, 255, 132, 255);
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