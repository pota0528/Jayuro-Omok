using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.tvOS;
using UnityEngine.UI;
using yu_namespace;

public class WinLosePanelController : MessagePopupController
{
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

    
    
    public void ShowCoinText(int coin)//찬영님이 주시는 데이터 형태로 넣기
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
            levelCount= 3;
        }
        return levelCount;
    }

    public bool SetResultPanel(int currentLevelCount, int levelPoint)
    {
        if (currentLevelCount + levelPoint < 0)
        {
            //TODO: 강등패널 띄우기
            Debug.Log("강등");
            if (YuConstants.level <= 18 && YuConstants.level >= 1)
            {
                YuConstants.level++;//급수가 클수록 레벨이 낮음
            }
            Destroy(gameObject);
            Debug.Log("사라짐");
            return false;
        }
        else if (currentLevelCount - Mathf.Abs(levelPoint) < 0)
        {
            //Todo: 승급패널 띄우기
            Debug.Log("승급");
            if (YuConstants.level<= 18 && YuConstants.level >= 1)
            {
                YuConstants.level--;//급수가 클수록 레벨이 낮음
            };
            Destroy(gameObject);
            return false;
          
        }
        else
        {
            if (YuConstants.isWin)//기준이 승급포인트
            {
                GetComponent<MessagePopupController>().Show("승리하였습니다!");
                levelPointResultText.text = currentLevelCount*2-(levelPoint+currentLevelCount)+"점을 얻으시면 승급됩니다.";
            }
            
            else 
            {
                GetComponent<MessagePopupController>().Show("패배하였습니다!");
                levelPointResultText.text = (levelPoint+currentLevelCount)+"점을 잃으시면 강등됩니다.";
            }
            
            plusGaugeBlocks = new GameObject[currentLevelCount];
            minusGaugeBlocks = new GameObject[currentLevelCount];
            minText.text = "-"+Mathf.Abs(currentLevelCount);
            maxText.text = "+"+Mathf.Abs(currentLevelCount);

            InitGaugeBlocks();//게이지바 초기화
            ColorGaugeBlocks();//게이지바 색칠하기
        }
        return true;
        
    }

    private void InitGaugeBlocks()//게이지바 초기화
    {
        for (int i = 0; i < levelCount; i++)
        {
            GameObject plusGaugeBlockObject = Instantiate(gaugeBlockPrefab, plusGaugeBlocksGroup);
            plusGaugeBlockObject.GetComponent<Image>().color= new Color32(217, 217, 217, 255);
            plusGaugeBlocks[i] = plusGaugeBlockObject;
        }

        for (int i = levelCount-1; i >= 0; i--)
        {
            GameObject minusGaugeBlockObject = Instantiate(gaugeBlockPrefab, minusGaugeBlocksGroup);
            minusGaugeBlockObject.GetComponent<Image>().color= new Color32(217, 217, 217, 255);
            minusGaugeBlocks[i] = minusGaugeBlockObject;
        }
    }
    
    private void ColorGaugeBlocks()//게이지바 색칠하기
    {
        //승점포인트 색칠하기
        for (int i = 0; i < Mathf.Abs(YuConstants.levelPoint); i++)
        {
            if (YuConstants.levelPoint > 0)
            {
                plusGaugeBlocks[i].GetComponent<Image>().color= Color.black;
            }
            else if (YuConstants.levelPoint < 0)
            {
                minusGaugeBlocks[i].GetComponent<Image>().color= Color.black;
            }
        }
    }
    
    public void OnClickRetryMatchButton()
    {
        Hide(() =>
        {
            //TODO: 메인씬+상점패널로 이동
            //게임씬 이동
            Debug.Log("게임씬으로 이동");
                
                
        });
    }
    
    
}
