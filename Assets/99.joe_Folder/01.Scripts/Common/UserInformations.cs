using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class UserInformations 
{
    
    [MenuItem("Window/PlayerPrefs Reset")]
    private static void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Reset Prefs");
    }
        
    
    
    private const string COIN_COUNT = "CoinCount";

    public static int CoinCount
    {
        get{return PlayerPrefs.GetInt(COIN_COUNT, 700);}
        set{PlayerPrefs.SetInt(COIN_COUNT, value);} 
    }
    
}




