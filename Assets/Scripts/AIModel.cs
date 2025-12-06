using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIModel {
    public const int MAX_DEPTH = 3; 
    
    public static Vector3Int nextMove(Game.BoardStorage board, bool cubeDiagonalsAllowed, int aiPlayerIndex) {
        int maxPoints = Int32.MinValue;
        Vector3Int bestMove = Vector3Int.zero;
        for (int x = 0; x < 3; ++x) {
            for (int y = 0; y < 3; ++y) {
                for (int z = 0; z < 3; ++z) {
                    if(board.hasPlayerIn(x, y, z)) continue;
                    int points = countPoints(board, cubeDiagonalsAllowed, true, aiPlayerIndex, x, y, z, 0);
                    if (points > maxPoints) {
                        bestMove = new Vector3Int(x, y, z);
                        maxPoints = points;
                    }

                    if (points == maxPoints && Random.value > 0.5f) {
                        bestMove = new Vector3Int(x, y, z);
                        maxPoints = points;
                    }
                }
            }
        }

        return bestMove;
    }

    private static int countPoints(Game.BoardStorage board, bool cubeDiagonalsAllowed, bool aiPlayerTurn, int aiPlayerIndex, int x, int y, int z, int depth) {
        if(board.hasPlayerIn(x, y, z)) return 0;
        board.set(x, y, z, aiPlayerTurn ? aiPlayerIndex : 1-aiPlayerIndex);
        if (board.checkForWinOpt(cubeDiagonalsAllowed, x, y, z)) {
            board.set(x, y, z, -1);
            return aiPlayerTurn ? 1 : -1;
        }

        if (depth + 1 > MAX_DEPTH) {
            board.set(x, y, z, -1);
            return 0;
        }

        int points = 0;
        for (int nx = 0; nx < 3; ++nx) {
            for (int ny = 0; ny < 3; ++ny) {
                for (int nz = 0; nz < 3; ++nz) {
                    points += countPoints(board, cubeDiagonalsAllowed, !aiPlayerTurn, aiPlayerIndex, nx, ny, nz, depth+1);
                }
            }
        }
        
        board.set(x, y, z, -1);
        return points;
    }

}