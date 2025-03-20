using park_namespace;
using TMPro;
using UnityEngine;


    public struct SignUpData
{
    public string nickname;
    public string id;
    public string password;
   
}
public class SignUpPanelController : PanelController
{
    [SerializeField] private TMP_InputField _idInputField;
    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_InputField _confirmPasswordInputField;

    private void SaveSignUpDataToPlayerPrefs(SignUpData signUpData)
    {
        PlayerPrefs.SetString("NickName", signUpData.nickname);
        PlayerPrefs.SetString("ID", signUpData.id);
        PlayerPrefs.SetString("Password", signUpData.password);
        PlayerPrefs.Save();
    }
    public void OnClickConfirmButton()
    {
        var id= _idInputField.text;
        var nickname = _nicknameInputField.text;
        var password = _passwordInputField.text;
        var confirmPassword = _confirmPasswordInputField.text;
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(nickname) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            //TODO: 입력값이 비어있음을 알리는 팝업창 표시 
            return;
        }
        
        //TODO: 플레이어 정보 받아와서 패스워드 동일한지, 이미 존재하는 아이디인지 

        if (password.Equals(confirmPassword))
        {
            SignUpData signUpData = new SignUpData
            {
                id = id,
                nickname = nickname,
                password = password

            };
            // signUpData.id = id;
            // signUpData.nickname = nickname;
            // signUpData.password = password;
            SaveSignUpDataToPlayerPrefs(signUpData);
            Debug.Log("회원가입 데이터 저장");
            // Destroy(gameObject);
            // GameManager.Instance.OpenLoginPanel();
            //TODO: 회원가입 진행 
        }
        else
        {
            //GameManeger.Instance.OpenConfirmPanel("비밀번호가 서로 다릅니다.", () =>
            // {
            //   _passwordInputField.text = "";
            //_confirmPasswordInputField.text = "";
            //  });
        }
    }

    public void OnClickBackButton()
    {
        Debug.Log("BackButton누름!");
        Destroy(gameObject);
        GameManager.Instance.OpenLoginPanel();
        //TODO: 뒤로가기 누르면 로그인 패널이 나오게 하기 
    }
}


