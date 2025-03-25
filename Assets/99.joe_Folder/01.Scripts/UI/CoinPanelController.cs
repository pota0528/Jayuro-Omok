using System.Collections;
using System.Collections.Generic;
using Joe_namespace;
using TMPro;
using UnityEngine;


public class CoinPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text _coinCountText;

    private int _coinCount;


    private void Start()
    {
        InitCoinCount(UIManager.Instance.coinCount);
    }

    public void InitCoinCount(int coinCount)
    {
        _coinCount = coinCount;
        _coinCountText.text = _coinCount.ToString();
    }
        
}

