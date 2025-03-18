using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ReplayUIController : MonoBehaviour
{
    [SerializeField] private Button nextButton, prevButton, firstButton, lastButton, exitButton;
    [SerializeField] private ReplayController replayController;
    [SerializeField] private MatchListController matchListController;
    [SerializeField] private GameObject matchListPanel, replayPanel;

    private void Start()
    {
        // 제어 버튼 연결
        nextButton.onClick.AddListener(replayController.NextMove);
        prevButton.onClick.AddListener(replayController.PreviousMove);
        firstButton.onClick.AddListener(replayController.FirstMove);
        lastButton.onClick.AddListener(replayController.LastMove);
        exitButton.onClick.AddListener(() =>
        {
            replayPanel.SetActive(false);
            matchListPanel.SetActive(true);
        });

        // 매치 선택 이벤트 연결
        matchListController.MatchSelected += match =>
        {
            replayController.LoadMatch(match);
            matchListPanel.SetActive(false);
            replayPanel.SetActive(true);
        };
    }
}