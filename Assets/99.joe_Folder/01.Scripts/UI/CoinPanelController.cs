using System.Collections;
using System.Collections.Generic;
using Joe_namespace;
using TMPro;
using UnityEngine;


public class CoinPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text _coinCountText;

    private int _coinCount;
    private PlayerData _playerData;


    private void Start()
    {
        _playerData = UserSessionManager.Instance.GetPlayerData();
        InitCoinCount(_playerData.coin);
    }

    public void InitCoinCount(int coinCount)
    {
        _coinCount = coinCount;
        _coinCountText.text = $"{_coinCount.ToString()}냥";
    }
        
}

