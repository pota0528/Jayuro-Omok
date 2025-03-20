using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Joe_namespace
{
    public class RankingPanelController : PanelController
    {
        
        //[SerializeField] private float cellHeight;
        
        [SerializeField] private GameObject ScrollView;
        
        private ScrollRect _scrollViewScrollRect;
        private RectTransform _scrollViewRectTransform;
        //
        // private List<Item> _items;
        // private LinkedList<RankingCellPanel> _visibleCells;
        
        public void Awake()
        {
            base.Awake();
            _scrollViewScrollRect = ScrollView.GetComponent<ScrollRect>();
            _scrollViewRectTransform = ScrollView.GetComponent<RectTransform>();
        }
        

        
        
        private void Start()
        {
            SetTitleText("RANKING");
            //LoadData();
        }
        
        public void OnClickCloseButton()
        {
            HidePanel();
        }
    }

}


