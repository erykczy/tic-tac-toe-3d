using UnityEngine;

public abstract class Menu : MonoBehaviour {
    public Vector3 getCameraFocus() {
        return transform.position;
    }

    public virtual void toggle(bool value) {
        transform.GetChild(0).gameObject.SetActive(value);
    }

    public bool isEnabled() {
        return transform.GetChild(0).gameObject.activeSelf;
    }
}
