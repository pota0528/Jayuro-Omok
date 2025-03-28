using park_namespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SignUpPanelController : PanelController
{
    [SerializeField] private TMP_InputField _idInputField;
    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_InputField _confirmPasswordInputField;

    [SerializeField] private Sprite OpenEye;
    [SerializeField] private Sprite CloseEye;

    [SerializeField] private Image PassWordButtonImage;
    [SerializeField] private Image PasswordConfirmButtonImage;
    [SerializeField] private Button profileButton;
    private DBManager _mongoDBManager;

    void Start()
    {
        _mongoDBManager = FindObjectOfType<DBManager>();
        // DBManager가 제대로 연결되었는지 확인
        if (_mongoDBManager == null)
        {
            Debug.LogError("DBManager가 하이어라키에 없습니다!");
        }
    }

    public void OnClickConfirmButton()
    {
        var id= _idInputField.text;
        var nickname = _nicknameInputField.text;
        var password = _passwordInputField.text;
        var confirmPassword = _confirmPasswordInputField.text;
        int selectedImageIndex = UIManager.Instance.GetProfileImageIndex(); //사용자가 선택한 이미지 
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(nickname) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            //TODO: 입력값이 비어있음을 알리는 팝업창 표시 
            return;
        }
        if (string.IsNullOrEmpty(id))
        {
            //TODO: // ID입력하세요 팝업창
            return;
        }

        if (!password.Equals(confirmPassword))
        {
            Debug.Log("비밀번호 일치X ");
            return;
        }
        
        //새로운 PlayerData 생성 
        PlayerData newPlayer = new PlayerData
        {
            id = id,
            nickname = nickname,
            password = password,
            level = 18,
            levelPoint = 0,
            coin = 1000,
            win = 0,
            lose = 0,
            imageIndex =selectedImageIndex
        };
        
        //MongoDB에 저장
        _mongoDBManager.RegisterPlayer(newPlayer);
        Debug.Log("회원가입 완료: "+newPlayer.nickname);
        

    }
    public void UpdateProfileImage(Sprite newProfileImage)
    {
        // UserPanel의 프로필 이미지 갱신
        profileButton.GetComponent<Image>().sprite = newProfileImage;
            
    }

    public void OnClickProfileButton()
    {
      
        UIManager.Instance.OpenProfilePanel();
    }

    public void OnClickBackButton()
    {
        Debug.Log("BackButton누름!");
        Destroy(gameObject);
        UIManager.Instance.OpenLoginPanel();
        //TODO: 뒤로가기 누르면 로그인 패널이 나오게 하기 
    }

    #region 로그인할때 비밀번호 눈 켜졌다꺼졌따
    public void OnClickEye1()
    {
        if (_passwordInputField != null)
        {
            if (_passwordInputField.contentType == TMP_InputField.ContentType.Password)
            {
                PassWordButtonImage.sprite = OpenEye;
                PassWordButtonImage.rectTransform.sizeDelta = new Vector2(44,30);
                _passwordInputField.contentType = TMP_InputField.ContentType.Standard;
            }
            else
            {
                PassWordButtonImage.sprite = CloseEye;
                PassWordButtonImage.rectTransform.sizeDelta = new Vector2(44,36);
                _passwordInputField.contentType = TMP_InputField.ContentType.Password;
            }
            _passwordInputField.ForceLabelUpdate();
        }
    }
    public void OnClickEye2()
    {
        if (_confirmPasswordInputField != null)
        {
            if (_confirmPasswordInputField.contentType == TMP_InputField.ContentType.Password)
            {
                PasswordConfirmButtonImage.sprite = OpenEye;
                PasswordConfirmButtonImage.rectTransform.sizeDelta = new Vector2(44,30);
                _confirmPasswordInputField.contentType = TMP_InputField.ContentType.Standard;
            }
            else
            {
                PasswordConfirmButtonImage.sprite = CloseEye;
                PasswordConfirmButtonImage.rectTransform.sizeDelta = new Vector2(44,36);
                _confirmPasswordInputField.contentType = TMP_InputField.ContentType.Password;
            }
            _confirmPasswordInputField.ForceLabelUpdate();
        }
    }
    

    #endregion
    
}


