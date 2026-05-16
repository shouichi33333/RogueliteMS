using UnityEngine;
using UnityEngine.InputSystem;

namespace jugyou.batoru.Player
{
    public class PlayerController : MonoBehaviour
    {
        private const float moveSpeed = 5f; //変更不可のスピード

        private const float rotateSpeed = 10;

        private const float laserMaxDistance = 50f;

        [SerializeField] private Rigidbody RB;  //リジットボディ

        [SerializeField] private Transform weponOrigin;

        [SerializeField] private LineRenderer laserLineRenderer;

        private PlayerInptActions inputActions;

        private Vector2 moveInput = Vector2.zero;

        private Vector3 moveDirection = Vector3.zero;   //移動方向

        private Transform mainCameraTra;
        public Vector3 CurrentVelocity { get; private set; }   //現在のベロシティを引き取れる

        private void Awake()
        {
            inputActions = new PlayerInptActions();
            inputActions.Player.Fire.performed += Fire;

            if(UnityEngine.Camera.main != null)
            {
                mainCameraTra = UnityEngine.Camera.main.transform;
            }
            else
            {
                Debug.LogError("カメラがないよ");
            }
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
            moveInput = inputActions.Player.Move.ReadValue<Vector2>();
            DrawLaserPointer();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            if (RB == null)
            {
                Debug.LogError("リジットボディがついていないよ");
                return;
            }
            if (moveInput == Vector2.zero)
            {
                RB.linearVelocity = new Vector3(0f, RB.linearVelocity.y, 0f);
                CurrentVelocity = Vector3.zero;
                return;
            }

            Vector3 cameraForward = mainCameraTra.forward;
            Vector3 cameraRight = mainCameraTra.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            RB.rotation = Quaternion.Slerp(RB.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            Vector3 targetVelocity = moveDirection * moveSpeed;
            RB.linearVelocity = new Vector3(targetVelocity.x,RB.linearVelocity.y, targetVelocity.z);

            CurrentVelocity = RB.linearVelocity;
        }

        private void Fire(InputAction.CallbackContext context)
        {
            Debug.Log("test");
        }
        private void DrawLaserPointer()
        {
            if(laserLineRenderer == null || weponOrigin == null || mainCameraTra == null)
            {
                return;
            }
            laserLineRenderer.SetPosition(0,weponOrigin.position);

            Ray ray = new Ray(mainCameraTra.position, mainCameraTra.forward);
            if(Physics.Raycast(ray, out RaycastHit hitInfo, laserMaxDistance))
            {
                if (hitInfo.collider.gameObject.CompareTag("Player"))
                {
                    laserLineRenderer.SetPosition(1, ray.GetPoint(laserMaxDistance));
                }
                else
                {
                    laserLineRenderer.SetPosition(1, hitInfo.point);
                }
            }
            else
            {
                laserLineRenderer.SetPosition(1, ray.GetPoint(laserMaxDistance));
            }
        }
    }
}