using System;
using UnityEngine;

public class MenuToggleVisuals : MonoBehaviour {
    private Animator m_animator;
    private MenuToggle m_toggle;
    
    private void Awake() {
        m_animator = GetComponent<Animator>();
        m_toggle = GetComponentInParent<MenuToggle>();
    }

    private void OnEnable() {
        m_animator.SetBool("Toggled", m_toggle.value);
    }

    private void OnMouseDown() {
        m_animator.SetBool("Toggled", m_toggle.value);
    }

    private void OnMouseEnter() {
        m_animator.SetBool("Highlighted", true);
    }

    private void OnMouseExit() {
        m_animator.SetBool("Highlighted", false);
    }
}
