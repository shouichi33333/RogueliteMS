using UnityEngine;

namespace jugyou.batoru.Camera
{

    public class cameraController : MonoBehaviour
    {
        private float lookSensitivity = 0.2f;

        private float distance = 5;

        private float heightOffset = 1.5f;

        private float minPitch = 10;

        private float maxPitch = 60;

        [SerializeField] private Transform target;

        private PlayerInptActions inputActions;

        private Vector2 lookInput = Vector2.zero;

        private float currentYaw = 0;

        private float currentPitch = 20;

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
            Vector3 targetPosi = target.position + Vector3.up * heightOffset;
            Quaternion rotate = Quaternion.Euler(currentPitch, currentYaw, 0f);

            //注意点から、計算した角度から後ろ方向へ距離分だけ離した位置を計算
            Vector3 cameraPosi = targetPosi - (rotate * Vector3.forward * distance);

            transform.position = cameraPosi;
            transform.rotation = rotate;
        }
    }
}