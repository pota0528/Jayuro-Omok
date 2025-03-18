using UnityEngine;
using UnityEngine.UI;

namespace kim_namespace
{
    public class OmokBlock : MonoBehaviour
    {
        [SerializeField] private Image markerImage; // 돌을 표시할 이미지
        [SerializeField] private Sprite blackStoneSprite; // 흑돌
        [SerializeField] private Sprite whiteStoneSprite; // 흰돌

        public enum MarkerType
        {
            None,
            Black,
            White
        } // 돌 타입 정의

        private Button button;
        private int row, col;

        // 초기화 함수: 행, 열 위치와 클릭 이벤트 설정
        public void Init(int row, int col, System.Action<int, int> onClick)
        {
            this.row = row;
            this.col = col;
            button = GetComponent<Button>();
            button.onClick.AddListener(() => onClick(row, col));
            SetMarker(MarkerType.None); // 처음엔 빈 칸
        }

        // 돌 표시 함수
        public void SetMarker(MarkerType markerType)
        {
            switch (markerType)
            {
                case MarkerType.Black:
                    markerImage.sprite = blackStoneSprite;
                    markerImage.color = new Color(1, 1, 1, 1); // 알파값을 1로 설정 (불투명하게)
                    break;
                case MarkerType.White:
                    markerImage.sprite = whiteStoneSprite;
                    markerImage.color = new Color(1, 1, 1, 1); // 알파값을 1로 설정 (불투명하게)
                    break;
                case MarkerType.None:
                    markerImage.sprite = null; // 빈 칸
                    markerImage.color = new Color(1, 1, 1, 0); // 알파값을 0으로 설정 (투명하게)
                    break;
            }
        }
    }
}