using UnityEngine;

namespace jugyou.batoru.Camera
{

    public class cameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;

        [Header("カメラの基本設定")]
        [SerializeField] float lookSensitivity = 0.2f;

        [SerializeField] float minPitch = -10;

        [SerializeField] float maxPitch = 60;

        [SerializeField] float zoomSpeed = 5;

        [Header("カメラの視点")]
        [SerializeField] float targetDistance = 2;

        [SerializeField] float targetHeightOffset = 1;

        [SerializeField] float targetShoulderOffset = 0.5f;

        private PlayerInptActions inputActions;

        private Vector2 lookInput = Vector2.zero;

        private float currentYaw = 0;

        private float currentPitch = 20;

        private float currentDistance = 0;

        private float currentHeightOffset = 0;

        private float currentShouldereOffset = 0;

        private void Awake()
        {
            inputActions = new PlayerInptActions();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void OnEnable()
        {
            inputActions.Enable();
        }
        private void OnDisable()
        {
            inputActions.Disable();
        }
        private void Update()
        {
            lookInput = inputActions.Player.Look.ReadValue<Vector2>();

            currentYaw += lookInput.x * lookSensitivity;
            currentPitch -= lookInput.y * lookSensitivity;

            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        }
        private void LateUpdate()
        {
            if (target == null) return;

            currentDistance = Mathf.Lerp(currentDistance, targetDistance, zoomSpeed * Time.deltaTime);
            currentHeightOffset = Mathf.Lerp(currentHeightOffset,targetHeightOffset, zoomSpeed * Time.deltaTime);
            currentShouldereOffset = Mathf.Lerp(currentShouldereOffset,targetShoulderOffset,zoomSpeed * Time.deltaTime);

            Quaternion rotate = Quaternion.Euler(currentPitch, currentYaw, 0f);

            Vector3 basePosition = target.position + Vector3.up * currentHeightOffset;

            Vector3 shoulderPosition = basePosition + (rotate * Vector3.right * currentShouldereOffset);

            //注意点から、計算した角度から後ろ方向へ距離分だけ離した位置を計算
            Vector3 cameraPosition = shoulderPosition + (rotate * Vector3.forward * currentDistance);

            transform.position = cameraPosition;
            transform.rotation = rotate;
        }
    }
}