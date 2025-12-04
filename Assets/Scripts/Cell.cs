using System;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
    public Vector3Int cellGamePosition;
    
    private void OnMouseDown() {
        Game.instance().claimPosition(cellGamePosition.x, cellGamePosition.y, cellGamePosition.z);
    }
}
