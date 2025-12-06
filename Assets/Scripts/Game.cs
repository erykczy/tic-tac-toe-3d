using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {
    public UnityEvent<WinEvent> onWin;
    public UnityEvent onReset;
    [SerializeField] private List<Player> m_players;
    private int m_currentPlayerIndex = 0;
    private BoardStorage m_board = new BoardStorage();
    private bool m_running = false;
    private bool m_cubeDiagonalsAllowed = false;
    private bool m_aiEnabled = false;
    private bool m_aiFirstTurn = false;
    private static Game s_game;
    
    public static Game instance() {
        return s_game;
    }

    public void reset() {
        m_currentPlayerIndex = 0;
        m_board.clear();
        onReset.Invoke();
    }

    public void start() {
        m_running = true;
        if(m_aiEnabled && m_aiFirstTurn)
            claimPositionByAI();
    }

    public void toggleCubeDiagonals(bool value) {
        m_cubeDiagonalsAllowed = value;
    }

    public void toggleAI(bool value) {
        m_aiEnabled = value;
    }

    public void toggleAIFirstTurn(bool value) {
        m_aiFirstTurn = value;
    }
    
    public void switchToNextPlayer() {
        ++m_currentPlayerIndex;
        if (m_currentPlayerIndex >= m_players.Count)
            m_currentPlayerIndex = 0;
        
        if (m_aiEnabled && m_currentPlayerIndex == (m_aiFirstTurn ? 0 : 1)) {
            claimPositionByAI();
        }
    }

    public void claimPositionByAI() {
        Vector3Int move = AIModel.nextMove(m_board, m_cubeDiagonalsAllowed, getCurrentPlayerIndex());
        claimPosition(move.x, move.y, move.z);
    }

    public Player getCurrentPlayer() {
        return m_players[m_currentPlayerIndex];
    }

    public int getCurrentPlayerIndex() {
        return m_currentPlayerIndex;
    }

    public bool isRunning() {
        return m_running;
    }
    
    public Player getPlayerInCell(int x, int y, int z) {
        return m_board.hasPlayerIn(x, y, z) ? m_players[m_board.get(x, y, z)] : null;
    }

    public bool hasPlayerInCell(int x, int y, int z) {
        return m_board.hasPlayerIn(x, y, z);
    }
    
    public void claimPosition(int x, int y, int z) {
        if (!isRunning()) return; 
        if (hasPlayerInCell(x, y, z)) return;
        m_board.set(x, y, z, m_currentPlayerIndex);
        List<Vector3Int> victoryPositions = new List<Vector3Int>();
        if (m_board.checkForWin(victoryPositions, m_cubeDiagonalsAllowed, x, y, z)) {
            triggerWin(x, y, z, victoryPositions);
            return;
        }
        switchToNextPlayer();
    }

    private void triggerWin(int x, int y, int z, List<Vector3Int> victoryPositions) {
        m_running = false;
        Player player = getPlayerInCell(x, y, z);
        onWin.Invoke(new WinEvent(player, m_board.get(x, y, z), new Vector3Int(x, y, z), victoryPositions));
    }
    
    private void Awake() {
        s_game = this;
    }

    public class BoardStorage {
        private int[,,] m_board;

        public BoardStorage() {
            clear();
        }

        public int get(int x, int y, int z) {
            return m_board[x, y, z] - 1;
        }

        public bool hasPlayerIn(int x, int y, int z) {
            return get(x, y, z) != -1;
        }

        public void set(int x, int y, int z, int i) {
            m_board[x, y, z] = i + 1;
        }

        public void clear() {
            m_board = new int[3, 3, 3];
        }

        public bool checkForWin(bool cubeDiagonalsAllowed, int x, int y, int z) {
            return checkForWin(null, cubeDiagonalsAllowed, x, y, z);
        }
        
        public bool checkForWin(List<Vector3Int> victoryPositions, bool cubeDiagonalsAllowed, int x, int y, int z) {
            List<Func<int, Vector3Int>> winFunctions = new List<Func<int, Vector3Int>>() {
                i => new Vector3Int(i, y, z), // x-axis
                i => new Vector3Int(x, i, z), // y-axis
                i => new Vector3Int(x, y, i), // z-axis
                
                i => new Vector3Int(i, i, z), // xy-plane first diagonal
                i => new Vector3Int(i, 2-i, z), // xy-plane second diagonal
                i => new Vector3Int(i, y, i), // xz-plane first diagonal
                i => new Vector3Int(i, y, 2-i), // xz-plane second diagonal
                i => new Vector3Int(x, i, i), // zy-plane first diagonal
                i => new Vector3Int(x, i, 2-i), // zy-plane second diagonal
            };
            if (cubeDiagonalsAllowed) {
                winFunctions.AddRange(new List<Func<int, Vector3Int>>() {
                    i => new Vector3Int(i, i, i), // cube diagonal
                    i => new Vector3Int(i, 2-i, i), // cube diagonal
                    i => new Vector3Int(2-i, i, i), // cube diagonal
                    i => new Vector3Int(i, i, 2-i), // cube diagonal
                });
            }

            bool win = false;
            foreach(var func in winFunctions) {
                win |= checkForWin(victoryPositions, x, y, z, func);
            }
            return win;
        }

        public bool checkForWin(List<Vector3Int> victoryPositions, int x, int y, int z, Func<int, Vector3Int> func) {
            Vector3Int originalPos = new Vector3Int(x, y, z);
            Vector3Int pos0 = func(0);
            Vector3Int pos1 = func(1);
            Vector3Int pos2 = func(2);
            if (pos0 != originalPos && pos1 != originalPos && pos2 != originalPos) {
                return false;
            }
            if (get(pos0.x, pos0.y, pos0.z) == get(pos1.x, pos1.y, pos1.z) && get(pos0.x, pos0.y, pos0.z) == get(pos2.x, pos2.y, pos2.z)) {
                if (victoryPositions != null) {
                    victoryPositions.Add(pos0);
                    victoryPositions.Add(pos1);
                    victoryPositions.Add(pos2);
                }
                return true;
            }
            return false;
        }
        
        public bool checkForWinOpt(bool cubeDiagonalsAllowed, int x, int y, int z) {
            if (threequal(m_board[0, y, z], m_board[1, y, z], m_board[2, y, z])) return true; // x-axis
            if (threequal(m_board[x, 0, z], m_board[x, 1, z], m_board[x, 2, z])) return true; // y-axis
            if (threequal(m_board[x, y, 0], m_board[x, y, 1], m_board[x, y, 2])) return true; // z-axis

            if (x == y && threequal(m_board[0, 0, z], m_board[1, 1, z], m_board[2, 2, z])) return true; // xy-plane first diagonal
            if (x == 2-y && threequal(m_board[0, 2, z], m_board[1, 1, z], m_board[2, 0, z])) return true; // xy-plane second diagonal
            if (x == z && threequal(m_board[0, y, 0], m_board[1, y, 1], m_board[2, y, 2])) return true; // xz-plane first diagonal
            if (x == 2-z && threequal(m_board[0, y, 2], m_board[1, y, 1], m_board[2, y, 0])) return true; // xz-plane second diagonal
            if (y == z && threequal(m_board[x, 0, 0], m_board[x, 1, 1], m_board[x, 2, 2])) return true; // zy-plane first diagonal
            if (y == 2-z && threequal(m_board[x, 0, 2], m_board[x, 1, 1], m_board[x, 2, 0])) return true; // zy-plane first diagonal

            if (cubeDiagonalsAllowed) {
                if (threequal(x, y, z) && threequal(m_board[0, 0, 0], m_board[1, 1, 1], m_board[2, 2, 2])) return true;
                if (threequal(x, 2-y, z) && threequal(m_board[0, 2, 0], m_board[1, 1, 1], m_board[2, 0, 2])) return true;
                if (threequal(2-x, y, z) && threequal(m_board[2, 0, 0], m_board[1, 1, 1], m_board[0, 2, 2])) return true;
                if (threequal(x, y, 2-z) && threequal(m_board[0, 0, 2], m_board[1, 1, 1], m_board[2, 2, 0])) return true;
            }

            return false;
        }
        
        public bool threequal(int a, int b, int c) {
            return a == b && b == c;
        }
    }

    public class WinEvent {
        public Player player;
        public int playerIndex;
        public Vector3Int position;
        public List<Vector3Int> victoryPositions;

        public WinEvent(Player player, int playerIndex, Vector3Int position, List<Vector3Int> victoryPositions) {
            this.player = player;
            this.playerIndex = playerIndex;
            this.position = position;
            this.victoryPositions = victoryPositions;
        }
    }
}
