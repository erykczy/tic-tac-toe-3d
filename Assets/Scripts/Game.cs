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
    }

    public void toggleCubeDiagonals(bool value) {
        m_cubeDiagonalsAllowed = value;
    }

    public void toggleAI(bool value) {
        m_aiEnabled = value;
    }
    
    public void switchToNextPlayer() {
        ++m_currentPlayerIndex;
        if (m_currentPlayerIndex >= m_players.Count)
            m_currentPlayerIndex = 0;
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
        if (checkForWin(victoryPositions, x, y, z)) {
            triggerWin(x, y, z, victoryPositions);
            return;
        }
        switchToNextPlayer();
    }

    /*private void checkForWin(int x, int y, int z) {
        bool win = false;
        win |= checkForWin(x, y, z, i => m_board.get(i - 1, y, z) == m_board.get(i, y, z)); // x-axis
        win |= checkForWin(x, y, z, i => m_board.get(x, i - 1, z) == m_board.get(x, i, z)); // y-axis
        win |= checkForWin(x, y, z, i => m_board.get(x, y, i - 1) == m_board.get(x, y, i)); // z-axis
        
        if(x == y)
            win |= checkForWin(x, y, z, (i, j) => m_board.get(i, i, z) == m_board.get(j, j, z)); // xy-plane first diagonal
        if(x == 2-y)
            win |= checkForWin(x, y, z, (i, j) => m_board.get(i, 2-i, z) == m_board.get(j, 2-j, z)); // xy-plane second diagonal
        if(x == z)
            win |= checkForWin(x, y, z, (i, j) => m_board.get(i, y, i) == m_board.get(j, y, j)); // xz-plane first diagonal
        if(x == 2-z)
            win |= checkForWin(x, y, z, (i, j) => m_board.get(i, y, 2-i) == m_board.get(j, y, 2-j)); // xz-plane second diagonal
        if(y == z)
            win |= checkForWin(x, y, z, (i, j) => m_board.get(x, i, i) == m_board.get(x, j, j)); // zy-plane first diagonal
        if(y == 2-z)
            win |= checkForWin(x, y, z, (i, j) => m_board.get(x, i, 2-i) == m_board.get(x, j, 2-j)); // zy-plane second diagonal
        
        
        if(x == y && y == z)
            win |= checkForWin(x, y, z, (i, j) => m_board.get(i, i, i) == m_board.get(j, j, j)); // cube diagonal
        if(x == z && y == 2-x)
            win |= checkForWin(x, y, z, (i, j) => m_board.get(i, 2-i, i) == m_board.get(j, 2-j, j)); // cube diagonal
        if(y == z && x == 2-y)
            win |= checkForWin(x, y, z, (i, j) => m_board.get(2-i, i, i) == m_board.get(2-j, j, j)); // cube diagonal
        if(x == y && z == 2-x)
            win |= checkForWin(x, y, z, (i, j) => m_board.get(i, i, 2-i) == m_board.get(j, j, 2-j)); // cube diagonal
        
        if(win) triggerWin(x, y, z);
    }*/
    
    private bool checkForWin(List<Vector3Int> victoryPositions, int x, int y, int z) {
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
        if (m_cubeDiagonalsAllowed) {
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

    /*private bool checkForWin(int x, int y, int z, Func<int, bool> condition) {
        bool win = true;
        for (int i = 1; i < 3; ++i) {
            if(!condition(i)){ win = false; break; }
        }
        return win;
    }

    private bool checkForWin(int x, int y, int z, Func<int, int, bool> condition) {
        return checkForWin(x, y, z, i => condition(i - 1, i));
    }*/

    private bool checkForWin(List<Vector3Int> victoryPositions, int x, int y, int z, Func<int, Vector3Int> func) {
        Vector3Int originalPos = new Vector3Int(x, y, z);
        Vector3Int pos0 = func(0);
        Vector3Int pos1 = func(1);
        Vector3Int pos2 = func(2);
        if (pos0 != originalPos && pos1 != originalPos && pos2 != originalPos) {
            return false;
        }
        if (m_board.get(pos0.x, pos0.y, pos0.z) == m_board.get(pos1.x, pos1.y, pos1.z) && m_board.get(pos0.x, pos0.y, pos0.z) == m_board.get(pos2.x, pos2.y, pos2.z)) {
            victoryPositions.Add(pos0);
            victoryPositions.Add(pos1);
            victoryPositions.Add(pos2);
            return true;
        }
        return false;
    }

    private void triggerWin(int x, int y, int z, List<Vector3Int> victoryPositions) {
        m_running = false;
        Player player = getPlayerInCell(x, y, z);
        onWin.Invoke(new WinEvent(player, m_board.get(x, y, z), new Vector3Int(x, y, z), victoryPositions));
    }
    
    private void Awake() {
        s_game = this;
    }

    private class BoardStorage {
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
