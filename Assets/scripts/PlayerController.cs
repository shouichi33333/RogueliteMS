using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float moveSpeed = 5f; //変更不可のスピード

    [SerializeField] Rigidbody RB;  //リジットボディ

    private Vector3 moveDirection = Vector3.zero;   //移動方向
    public Vector3 CurrentVelocity {  get; private set; }   //現在のベロシティを引き取れる

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(x, 0, y);
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
        if (moveDirection == Vector3.zero)
        {
            RB.linearVelocity = new Vector3(0f, RB.linearVelocity.y, 0f);
            CurrentVelocity = Vector3.zero;
        }

        Vector3 targetVelocity = moveDirection * moveSpeed;
        RB.linearVelocity = new Vector3(targetVelocity.x, RB.linearVelocity.y, targetVelocity.z);
        CurrentVelocity = RB.linearVelocity;
    }
}
