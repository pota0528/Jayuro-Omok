using System.Collections.Generic;
using UnityEngine;
using lee_namespace;

namespace lee_namespace
{
    public class MCTSNode
    {
        public MCTSNode parent; // 부모 노드
        public List<MCTSNode> children; // 자식 노드 리스트
        public int visits; // 방문 횟수
        public double score; // 누적 점수
        public int row, col; // 이 노드가 나타내는 수의 좌표
        public GameManager.PlayerType[,] boardState; // 현재 보드 상태
        public GameManager.PlayerType currentPlayer;

        public MCTSNode(MCTSNode parent, int row, int col, GameManager.PlayerType[,] boardState,
            GameManager.PlayerType currentPlayer)
        {
            this.parent = parent;
            this.row = row;
            this.col = col;
            this.boardState = (GameManager.PlayerType[,])boardState.Clone(); 
            this.currentPlayer = currentPlayer;
            this.children = new List<MCTSNode>();
            this.visits = 0;
            this.score = 0;
        }

        public double GetUCB1()
        {
            if (visits == 0) return double.MaxValue; // 방문하지 않은 노드는 우선 탐색하기로..
            return score / visits + Mathf.Sqrt(2 * Mathf.Log(parent.visits) / visits);
        }
    }
}
