using System;
using UnityEngine;

public class MenuButtonVisuals : MonoBehaviour {
    private Animator m_animator;
    
    private void Awake() {
        m_animator = GetComponent<Animator>();
    }

    private void OnMouseEnter() {
        m_animator.SetBool("Highlighted", true);
    }

    private void OnMouseExit() {
        m_animator.SetBool("Highlighted", false);
    }
}
