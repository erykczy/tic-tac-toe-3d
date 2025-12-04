using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinMenu : MonoBehaviour {
    [SerializeField] private TextMeshPro m_titleText;
    [SerializeField] private List<Transform> m_buttons;
    [SerializeField] private List<Colors> m_colors;
    
    private void Start() {
        SceneDirector.instance().afterWin.AddListener(afterWin);
        Game.instance().onReset.AddListener(onReset);
    }

    private void afterWin(Game.WinEvent e) {
        transform.GetChild(0).gameObject.SetActive(true);
        Colors colors = m_colors[e.playerIndex];
        m_titleText.color = colors.titleColor;
        m_titleText.text = $"{e.player.name} won!";
        
        foreach(var button in m_buttons)
        {
            button.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", colors.buttonColor);
            button.GetComponentInChildren<MeshRenderer>().material.SetColor("_HighlightColor", colors.buttonHighlightColor);
            button.GetComponentInChildren<CubeText>().defaultColor = colors.buttonTextColor;
            button.GetComponentInChildren<CubeText>().highlightColor = colors.buttonTextHighlightColor;
        }
    }

    private void onReset() {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    [Serializable]
    public class Colors {
        public Color titleColor;
        public Color buttonColor;
        public Color buttonHighlightColor;
        public Color buttonTextColor;
        public Color buttonTextHighlightColor;
    }
}
