using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CubeText : MonoBehaviour {
    public String text;
    public Color defaultColor;
    public Color highlightColor;
    [Range(0.0f, 1.0f)] public float highlight;
    [SerializeField] private GameObject m_prefab;
    private List<TextMeshPro> m_texts = new List<TextMeshPro>();
    private static List<Vector3> s_directions = new List<Vector3>() {
        Vector3.forward,
        /*Vector3.back,
        Vector3.down,
        Vector3.up,
        Vector3.right,
        Vector3.left*/
    };
    
    private void Start() {
        foreach(var direction in s_directions) {
            m_texts.Add(Instantiate(m_prefab, transform.position, Quaternion.LookRotation(direction), transform).GetComponentInChildren<TextMeshPro>());
        }
    }

    private void Update() {
        foreach(var text in m_texts)
        {
            text.color = Color.Lerp(defaultColor, highlightColor, highlight);
            text.text = this.text;
        }
    }
}
