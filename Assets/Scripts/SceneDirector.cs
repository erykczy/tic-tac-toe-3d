using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SceneDirector : MonoBehaviour {
    public UnityEvent<Game.WinEvent> afterWin = new UnityEvent<Game.WinEvent>();
    private static SceneDirector m_instance;
    
    public static SceneDirector instance() {
        return m_instance;
    }

    private void Awake() {
        m_instance = this;
    }

    private void Start() {
        Game.instance().onWin.AddListener(e => StartCoroutine(onWin(e)));
    }

    private IEnumerator onWin(Game.WinEvent e) {
        yield return new WaitForSeconds(1);
        afterWin.Invoke(e);
    }
}
