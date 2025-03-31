using UnityEditor;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D defaultCursor;  // 기본 커서 이미지
    public Texture2D clickCursor;    // 클릭 시 커서 이미지
    private bool isClicking = false; // 클릭 상태 여부

    void Start()
    {
        // 기본 커서를 설정합니다.
        SetCursor(defaultCursor);
        Cursor.SetCursor(PlayerSettings.defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
   
        
    }

    void Update()
    {
        // 마우스 클릭 시 클릭 커서로 변경
        if (Input.GetMouseButtonDown(0))  // 왼쪽 마우스 버튼 클릭
        {
            SetCursor(clickCursor);
            isClicking = true;
        }

        // 마우스 버튼을 떼면 기본 커서로 복구
        if (Input.GetMouseButtonUp(0))  // 왼쪽 마우스 버튼 떼기
        {
            SetCursor(defaultCursor);
            isClicking = false;
        }
    }

    // 커서 이미지를 설정하는 함수
    void SetCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}