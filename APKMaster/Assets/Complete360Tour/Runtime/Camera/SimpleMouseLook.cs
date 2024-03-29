﻿using UnityEngine;

namespace Complete360Tour {
    [AddComponentMenu("Complete360Tour/Helpers/Complete360Tour")]
    public class SimpleMouseLook : MonoBehaviour {

        //-----------------------------------------------------------------------------------------
        // Inspector Variables:
        //-----------------------------------------------------------------------------------------
        [Header("Enabled Settings")] [SerializeField] protected bool enableInEditor = true;
        [SerializeField] protected bool enableInBuild = false;

        [Header("Cursor Settings")] [SerializeField] protected CursorLockMode lockMode;
        [SerializeField] protected bool hideCursor;
        [SerializeField] protected bool rightMouseButton = false;

        [Header("Parameters")] [SerializeField] protected float sensitivityMutliplier = 1;
        [SerializeField] protected float smoothingMultiplier = 1;

        //-----------------------------------------------------------------------------------------
        // Private Fields:
        //-----------------------------------------------------------------------------------------

        private Vector2 clampDegrees = new Vector2(360, 180);
        private Vector2 mouseAbs;
        private Vector2 mouseSmooth;
        private Vector2 sensitivity = new Vector2(0.1f, 0.125f);
        private Vector2 smoothing = new Vector2(10, 10);
        private Vector2 targetDirection;

        //-----------------------------------------------------------------------------------------
        // Unity Lifecycle:
        //-----------------------------------------------------------------------------------------

        protected void Awake() {
            smoothing *= smoothingMultiplier;
            sensitivity *= sensitivityMutliplier;

            if (enableInEditor) {
            }
            if (enableInBuild) {
            }

#if UNITY_EDITOR
            enabled = enableInEditor;
#else
			enabled = enableInBuild;
#endif

            Cursor.lockState = lockMode;
            Cursor.visible = !hideCursor;
        }

        protected void OnEnable() {
            targetDirection = transform.localRotation.eulerAngles;
            mouseAbs = Vector2.zero;
        }

        protected void Update() {
			Quaternion targetOrientation = Quaternion.Euler(targetDirection);

            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            if (rightMouseButton && !Input.GetMouseButton(1)) {
                mouseDelta = Vector2.zero;
            }

            if (smoothingMultiplier > 0) {
                mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

                mouseSmooth.x = Mathf.Lerp(mouseSmooth.x, mouseDelta.x, 1f / smoothing.x);
                mouseSmooth.y = Mathf.Lerp(mouseSmooth.y, mouseDelta.y, 1f / smoothing.y);

                mouseAbs += mouseSmooth;
            }
            else {
                mouseAbs += mouseDelta;
            }

            Quaternion xRotation = Quaternion.AngleAxis(-mouseAbs.y, targetOrientation * Vector3.right);
            transform.localRotation = xRotation;

            if (clampDegrees.y < 360) mouseAbs.y = Mathf.Clamp(mouseAbs.y, -clampDegrees.y * 0.5f, clampDegrees.y * 0.5f);

            transform.localRotation *= targetOrientation;

            Quaternion yRotation = Quaternion.AngleAxis(mouseAbs.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
    }
}