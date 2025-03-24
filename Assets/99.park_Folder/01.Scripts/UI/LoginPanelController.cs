using System;
using System.Collections;
using System.Collections.Generic;
using park_namespace;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

    public class LoginPanelController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _idInputField;
        [SerializeField] private TMP_InputField _passwordInputField;

        private  DBManager dbManager;

        private void Start()
        {
            dbManager = FindObjectOfType<DBManager>();
        }

        public void OnClickLogInButton()
        {
            Debug.Log("로그인 버튼 누름");
            string id = _idInputField.text;
            string password = _passwordInputField.text;
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
            {
                Debug.Log("아이디,비밀번호를 입력하지X");
                return;
            }
            if (dbManager == null) // dbManager가 null인 경우 처리
            {
                Debug.LogError("DBManager가 초기화되지 않았습니다");
                return;
            }

            //DBManager.Login 호출
            var (player, message) = dbManager.Login(id, password);

            if (player != null)
            {
                Debug.Log("로그인 성공"+player.nickname);
                //로그인한 유저 데이터를 UserSessionManager에 저장
                UserSessionManager.Instance.SetPlayerData(player);
                // 로그인한 유저 데이터를 UIManager에 저장
                UIManager.Instance.SetPlayerData(player);
               
                UIManager.Instance.OpenUserPanel();
            }
            else
            {
                Debug.Log("로그인 실패");
            }
            
        }

        public void OnClickSignUpButton()
        {
            Destroy(gameObject);
            UIManager.Instance.OpenSignUpPanel();
        }
    }

  
