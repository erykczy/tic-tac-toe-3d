using System;
using System.Collections.Generic;
using UnityEngine;

public class CellVisuals : MonoBehaviour {
    public Color defaultHoverBackgroundColor;
    [SerializeField] private MeshRenderer m_backgroundRenderer;
    private Transform m_model;
    private Cell m_cell;
    private Color m_defaultBackgroundColor;
    private Animator m_backgroundAnimator;

    public void highlight() {
        m_backgroundAnimator.SetTrigger("HighlightImpulse");
    }
    
    private void Start() {
        m_cell = GetComponentInParent<Cell>();
        m_defaultBackgroundColor = m_backgroundRenderer.material.GetColor("_Color");
        m_backgroundAnimator = m_backgroundRenderer.GetComponent<Animator>();
    }

    private void Update() {
        if (!m_model) {
            Player player = Game.instance().getPlayerInCell(m_cell.cellGamePosition.x, m_cell.cellGamePosition.y, m_cell.cellGamePosition.z);
            if (player != null) {
                m_model = Instantiate(player.prefab, transform.position, player.prefab.transform.rotation, transform).transform;
                m_backgroundRenderer.material.SetColor("_Color", player.cellBackgroundColor);
            }
        }
    }

    private void setHover(bool value) {
        if (!m_model) {
            m_backgroundRenderer.material.SetColor("_Color", value ? defaultHoverBackgroundColor : m_defaultBackgroundColor);
        }
    }

    private void OnMouseEnter() {
        if(!Game.instance().isRunning()) return;
        setHover(true);
    }

    private void OnMouseExit() {
        if(!Game.instance().isRunning()) return;
        setHover(false);       
    }
}
