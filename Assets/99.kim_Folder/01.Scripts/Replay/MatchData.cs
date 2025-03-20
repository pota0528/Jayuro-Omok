using System.Collections.Generic;
using System;

[System.Serializable]
public class Move
{
    public int row; // 행
    public int col; // 열
    public string color; // "흑" 또는 "백"
}

[System.Serializable]
public class MatchData
{
    public string title; // 유저 닉네임
    public string date; // 저장 날짜
    public List<Move> moves; // 이동 목록
}