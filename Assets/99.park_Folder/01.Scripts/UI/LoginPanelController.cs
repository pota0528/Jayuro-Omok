using System.Collections;
using System.Collections.Generic;
using park_namespace;
using TMPro;
using UnityEngine;

public struct LoginData
{
    public string id;
    public string password;
}

public class LoginPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _idInputField;
    [SerializeField] private TMP_InputField _passwordInputField;

    public void OnClickLogInButton()
    {
        Debug.Log("로그인 버튼 누름");
        string id = _idInputField.text;
        string password = _passwordInputField.text;
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            return;
        }

        var loginData = new LoginData();
        loginData.id = id;
        loginData.password = password;

        if (PlayerPrefs.GetString("ID") == id)
        {
            if (PlayerPrefs.GetString("Password") == password)
            {
                Debug.Log("로그인 성공");
                //TODO: 로그인 진심 구현 
                GameManager.Instance.OpenUserPanel();
            }
            else
            {
                Debug.Log("비밀번호가 틀렸습니다.");
            }
        }
     
      


        
        
    }

    public void OnClickSignUpButton()
    {
        Destroy(gameObject);
       GameManager.Instance.OpenSignUpPanel();
    }
}
