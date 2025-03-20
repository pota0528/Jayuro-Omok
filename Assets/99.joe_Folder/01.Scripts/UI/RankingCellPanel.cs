using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Joe_namespace
{
    public class RankingCellPanel : MonoBehaviour
    {
        //셀 패널에 프사와 닉네임, 급수등 
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private TextMeshProUGUI[] RankingIndexText;

        //public int Index { get; private set; }
        private RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform;
        
        private int _rankingIndex;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetRankingCell(int rankingIndex)
        {
            _rankingIndex = rankingIndex;

            foreach (var rankingIndexText in RankingIndexText)
            {
                var IndexText = _rankingIndex + 1;
                rankingIndexText.text = IndexText.ToString();
            }
            
        }
        
        


        // public void SetItem(Item item, int index)
        // {
        //     //리소스 파일안에 파일이름으로 가져옴
        //     //image.sprite = Resources.Load<Sprite>(item.imageFileName);
        //     titleText.text = item.title;
        //     subtitleText.text = item.subtitle;
        //
        //     Index = index;
        //
        // }
    }
}
