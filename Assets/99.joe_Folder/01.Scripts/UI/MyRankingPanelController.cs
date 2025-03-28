using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MyRankingPanelController : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI kdText;

    public void SetMyRankingData(PlayerData player, int rank)
    {
        nicknameText.text = player.nickname;
        levelText.text = $"Lv.{player.level}";
        rankText.text = $"{rank + 1}";
        kdText.text = $"승: {player.win}  패: {player.lose}";

        if (player.imageIndex >= 0)
            profileImage.sprite = UIManager.Instance.GetProfileImage(player.imageIndex);
    }
}