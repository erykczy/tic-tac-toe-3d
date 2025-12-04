using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour {
    [SerializeField] private float m_sensivitivity;
    [SerializeField] private float m_damping;
    [SerializeField] private float m_maxDot;
    [SerializeField] private float m_scrollSensivitivity;
    [SerializeField] private float m_distanceChangeSpeed;
    [SerializeField] private float m_distance;
    [SerializeField] private float m_minDistance;
    [SerializeField] private float m_maxDistance;
    [SerializeField] private float m_focusChangeTime;
    [SerializeField] private Vector3 m_defaultFocus;
    private Vector3 m_cameraVector = Vector3.back;
    private float m_targetDistance;
    private Vector3 m_targetFocus;
    private Vector3 m_focus;
    private Vector3 m_focusVelocity;
    private Vector2 m_prevPos;
    private Vector2 m_velocity;

    private void Start() {
        m_prevPos = Input.mousePosition;
        m_targetDistance = m_distance;
        m_targetFocus = m_defaultFocus;
        m_focus = m_defaultFocus;
    }

    private void Update() {
        m_targetFocus = Vector3.zero;
        foreach(var menu in FindObjectsByType<Menu>(FindObjectsSortMode.None))
        {
            if (menu.isEnabled()) {
                m_targetFocus = menu.getCameraFocus();
                break;
            }
        }
        
        if (Input.GetMouseButton(1)) {
            var mouseDelta = (Vector2)Input.mousePosition - m_prevPos;
            mouseDelta *= m_sensivitivity;
            mouseDelta.y = -mouseDelta.y;
            m_velocity = mouseDelta/Time.deltaTime;
        }
        
        float dot = Vector3.Dot(m_cameraVector, Vector3.up);
        if (dot > m_maxDot && m_velocity.y > 0) {
            m_velocity.y = 0;
        }
        else if (dot < -m_maxDot && m_velocity.y < 0) {
            m_velocity.y = 0;
        }
        m_prevPos = Input.mousePosition;
        m_cameraVector = Quaternion.AngleAxis(m_velocity.y * Time.deltaTime, transform.right) * m_cameraVector;
        m_cameraVector = Quaternion.AngleAxis(m_velocity.x * Time.deltaTime, Vector3.up) * m_cameraVector;
        m_cameraVector.Normalize();
        m_velocity *= Mathf.Pow(m_damping, Time.deltaTime);

        float scrollDelta = Input.mouseScrollDelta.y;
        m_targetDistance += -scrollDelta * m_scrollSensivitivity;
        m_targetDistance = Math.Clamp(m_targetDistance, m_minDistance, m_maxDistance);
        float maxDistanceDelta = m_distanceChangeSpeed * Time.deltaTime;
        m_distance += Math.Clamp(m_targetDistance - m_distance, -maxDistanceDelta, maxDistanceDelta);

        m_focus = Vector3.SmoothDamp(m_focus, m_targetFocus, ref m_focusVelocity, m_focusChangeTime, float.PositiveInfinity, Time.deltaTime);
        
        transform.position = m_focus + m_distance * m_cameraVector;
        transform.forward = -m_cameraVector;
    }
}
