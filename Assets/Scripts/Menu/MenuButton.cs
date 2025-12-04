using System;
using UnityEngine;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour {
    public UnityEvent ClickAction = new UnityEvent();
    
    private void OnMouseDown() {
        ClickAction.Invoke();
    }
}
