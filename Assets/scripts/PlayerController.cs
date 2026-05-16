using UnityEngine;
using UnityEngine.InputSystem;

namespace jugyou.batoru.Player
{
    public class PlayerController : MonoBehaviour
    {
        private const float moveSpeed = 5f; //変更不可のスピード

        [SerializeField] Rigidbody RB;  //リジットボディ

        private PlayerInptActions inputActions;

        private Vector2 moveInput = Vector2.zero;

        private Vector3 moveDirection = Vector3.zero;   //移動方向
        public Vector3 CurrentVelocity { get; private set; }   //現在のベロシティを引き取れる

        private void Awake()
        {
            inputActions = new PlayerInptActions();
            inputActions.Player.Fire.performed += Fire;
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

            Vector2 targetVelocity = new Vector2(moveInput.x, moveInput.y);
            targetVelocity.Normalize();
            Vector3 playerVelocity = new Vector3(targetVelocity.x, RB.linearVelocity.y, targetVelocity.y) * moveSpeed;
            RB.linearVelocity = playerVelocity;
            CurrentVelocity = RB.linearVelocity;
        }

        private void Fire(InputAction.CallbackContext context)
        {
            Debug.Log("test");
        }
    }
}