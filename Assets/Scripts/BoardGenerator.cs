using System;
using UnityEngine;

public class BoardGenerator : MonoBehaviour {
    [SerializeField] private GameObject m_cellPrefab;
    [SerializeField] private float m_cellGap;
    private Cell[,,] m_cellObjects;

    public void reset() {
        for(int x = 0; x < 3; ++x)
        {
            for(int y = 0; y < 3; ++y)
            {
                for(int z = 0; z < 3; ++z)
                {
                    Destroy(m_cellObjects[x, y, z].gameObject);
                }
            }
        }
        generate();
    }
    
    public void generate() {
        for (int x = -1; x <= 1; ++x) {
            for (int y = -1; y <= 1; ++y) {
                for (int z = -1; z <= 1; ++z) {
                    var position = new Vector3(x, y, z) * m_cellGap;
                    var cell = Instantiate(m_cellPrefab, transform.position + position, Quaternion.identity, transform).GetComponent<Cell>();
                    cell.cellGamePosition = new Vector3Int(x+1, y+1, z+1);
                    m_cellObjects[x + 1, y + 1, z + 1] = cell;
                }
            }
        }
    }

    public Cell getCell(int x, int y, int z) {
        return m_cellObjects[x, y, z];
    }

    private void Awake() {
        m_cellObjects = new Cell[3, 3, 3];
    }

    private void Start() {
        generate();
        Game.instance().onWin.AddListener(onWin);
        Game.instance().onReset.AddListener(reset);
    }

    private void onWin(Game.WinEvent e) {
        foreach (var pos in e.victoryPositions) {
            getCell(pos.x, pos.y, pos.z).GetComponent<CellVisuals>().highlight();
        }
    }
}
