using System;
using UnityEngine;
using UnityEngine.Events;

public class MenuToggle : MonoBehaviour {
    public bool value = false;
    public UnityEvent onToggleOn;
    public UnityEvent onToggleOff;
    private bool initalized = false;
    
    private void Start() {
        (value ? onToggleOn : onToggleOff).Invoke();
    }

    private void OnMouseDown() {
        value = !value;
        (value ? onToggleOn : onToggleOff).Invoke();
    }
}
