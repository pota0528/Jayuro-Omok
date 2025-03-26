using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.tvOS;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class WinLosePanelController : MessagePopupController
{
    [Header("Panel")]
    [SerializeField] private GameObject gaugeBlockPrefab;
    [SerializeField] private GameObject[] plusGaugeBlocks;
    [SerializeField] private GameObject[] minusGaugeBlocks;
    [SerializeField] private RectTransform plusGaugeBlocksGroup;
    [SerializeField] private RectTransform minusGaugeBlocksGroup;
    [SerializeField] private TextMeshProUGUI levelPointResultText;
    [SerializeField] private TextMeshProUGUI maxText;
    [SerializeField] private TextMeshProUGUI minText;
    [SerializeField] private TextMeshProUGUI coinText;
    
    private int levelCount;

    private PlayerData playerData;

    private void Awake()
    {
        playerData = UserSessionManager.Instance.GetPlayerData();

        winLosePanelObject.SetActive(false);
    }


    public void ShowCoinText(int coin) //찬영님이 주시는 데이터 형태로 넣기
    {
        coinText.text = coin.ToString();
    }

    

    public int GetLevelCount(int level)
    {
        if (level >= 1 && level <= 4)
        {
            levelCount = 10;
        }
        else if (level >= 5 && level <= 9)
        {
            levelCount = 5;
        }
        else if (level >= 10 && level <= 18)
        {
            levelCount = 3;
        }

        return levelCount;
    }

    public bool SetResultPanel(int currentLevelCount, GameManager.GameResult gameResult)
    {
        if (gameResult == GameManager.GameResult.Win)
        {
            playerData.levelPoint++;
            playerData.win++;
            UserSessionManager.Instance.SetPlayerData(playerData);
            DBManager.Instance.UpdatePlayerData(playerData);
            if (currentLevelCount - Mathf.Abs(playerData.levelPoint) <= 0)
            {
                //Todo: 승급패널 띄우기
                if (playerData.level>1)
                {
                    playerData.level--; //급수가 클수록 레벨이 낮음
                }
                Destroy(gameObject);
                UserSessionManager.Instance.SetPlayerData(playerData);
                DBManager.Instance.UpdatePlayerData(playerData);
                Debug.Log(playerData.level+"지금 레벨");
                return false;
            }
            else
            {
                GetComponent<MessagePopupController>().Show("게임에서 승리하였습니다.\n1승급 포인트를 받았습니다.");
                levelPointResultText.text =
                    currentLevelCount * 2 - (playerData.levelPoint + currentLevelCount) + "게임을 승리하면\n승급됩니다.";
                
                plusGaugeBlocks = new GameObject[currentLevelCount];
                minusGaugeBlocks = new GameObject[currentLevelCount];
                minText.text = "-" + Mathf.Abs(currentLevelCount);
                maxText.text = "+" + Mathf.Abs(currentLevelCount);
                
                InitGaugeBlocks(currentLevelCount); //게이지바 초기화
                ColorGaugeBlocks(); //게이지바 색칠하기
                
                UserSessionManager.Instance.SetPlayerData(playerData);
                DBManager.Instance.UpdatePlayerData(playerData);
                return true;
            }
        }
        
        if (gameResult == GameManager.GameResult.Lose)
        {
            playerData.levelPoint--;
            playerData.lose++;
            UserSessionManager.Instance.SetPlayerData(playerData);
            DBManager.Instance.UpdatePlayerData(playerData);
            if (currentLevelCount-Mathf.Abs(playerData.levelPoint) <= 0)
            {
                //todo: 강등패널띄우기
                if (playerData.level < 18)//[데이터 처리]-level
                {
                    playerData.level++; //급수가 클수록 레벨이 낮음, [데이터 처리]-level
                }
                Destroy(gameObject);
                UserSessionManager.Instance.SetPlayerData(playerData);
                DBManager.Instance.UpdatePlayerData(playerData);
                Debug.Log(playerData.level+"지금 레벨");
                return false;
            }
            else
            {
                GetComponent<MessagePopupController>().Show("게임에서 패배하였습니다.\n1승급 포인트를 잃었습니다.");
                levelPointResultText.text = (playerData.levelPoint + currentLevelCount) + "게임을 패배하면\n강등됩니다.";
            
                plusGaugeBlocks = new GameObject[currentLevelCount];
                minusGaugeBlocks = new GameObject[currentLevelCount];
                minText.text = "-" + Mathf.Abs(currentLevelCount);
                maxText.text = "+" + Mathf.Abs(currentLevelCount);
                
                UserSessionManager.Instance.SetPlayerData(playerData);
                DBManager.Instance.UpdatePlayerData(playerData);
            
                InitGaugeBlocks(currentLevelCount); //게이지바 초기화
                ColorGaugeBlocks(); //게이지바 색칠하기

                return true;
            }
        }

        return true;
    }

    private void InitGaugeBlocks(int currentLevelCount) //게이지바 초기화
    {
        for (int i = 0; i < currentLevelCount; i++)
        {
            GameObject plusGaugeBlockObject = Instantiate(gaugeBlockPrefab, plusGaugeBlocksGroup);
            plusGaugeBlockObject.GetComponent<Image>().color = new Color32(217, 217, 217, 255);
            plusGaugeBlocks[i] = plusGaugeBlockObject;
        }

        for (int i = currentLevelCount - 1; i >= 0; i--)
        {
            GameObject minusGaugeBlockObject = Instantiate(gaugeBlockPrefab, minusGaugeBlocksGroup);
            minusGaugeBlockObject.GetComponent<Image>().color = new Color32(217, 217, 217, 255);
            minusGaugeBlocks[i] = minusGaugeBlockObject;
        }
    }

    private void ColorGaugeBlocks() //게이지바 색칠하기
    {
        //승점포인트 색칠하기
        for (int i = 0; i < Mathf.Abs(playerData.levelPoint); i++)
        {
            if (playerData.levelPoint > 0)
            {
                plusGaugeBlocks[i].GetComponent<Image>().color = Color.black;
            }
            else if (playerData.levelPoint < 0)
            {
                minusGaugeBlocks[i].GetComponent<Image>().color = Color.black;
            }
        }
    }

    public void OnClickRetryMatchButton()
    {
        if (playerData.coin >= 100)
        {
            Hide(() =>
            {
                playerData.coin -= 100;
                SceneManager.LoadScene("Game");
            });
        }

        else
        {
            //todo: 코인 -100차감
            if (playerData.coin < 100)
            {
                UIManager.Instance.OpenNoCoinPanel();
                Debug.Log("노코인패널");
            }
        }
        
    }

    public void WinLosePanelConfirmButton()
    {
        base.OnClickExitButton();
    }
    
    private void OnDestroy()
    {
        //todo: 데이터 저장(코인, 급수, 승점포인트)
        UserSessionManager.Instance.SetPlayerData(playerData);
        DBManager.Instance.UpdatePlayerData(playerData);
        transform.DOKill();
        
    }

    
}