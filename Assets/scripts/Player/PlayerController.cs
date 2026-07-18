using Core.Interface;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using MasterData;
using jugyou.batoru.Enum;
namespace jugyou.batoru.Player
{
    public class PlayerController : MonoBehaviour
    {
        private const float moveSpeed = 5f; //変更不可のスピード

        private const float rotateSpeed = 10;

        private const float laserMaxDistance = 50f;

        private const float ATTACK_RANGE = 50f;

        [SerializeField] private Rigidbody RB;  //リジットボディ

        [SerializeField] private Transform weponOrigin;

        [SerializeField] private LineRenderer laserLineRenderer;

        [SerializeField] private ulong weaponId = 1;

        [SerializeField] ParticleSystem muzzleFlash;

        private WeaponDataRecord WeaponData;

        private PlayerInptActions inputActions;

        private Vector2 moveInput = Vector2.zero;

        private Vector3 moveDirection = Vector3.zero;   //移動方向

        private Transform mainCameraTra;

        private bool isReloding;

        private bool canShot = true;

        private CancellationTokenSource fireCT;
        public Vector3 CurrentVelocity { get; private set; }   //現在のベロシティを引き取れる

        public int CurrntAmmo { get; private set; }

        private void Awake()
        {
            gameObject.SetActive(false);
        }
        public void Setup()
        {
            WeaponData = MasterDataAccessor.Instance.GetById<WeaponDataRecord>(weaponId);
            if (WeaponData == null)
            {
                Debug.LogError("SOついてない");
                return;
            }
            CurrntAmmo = WeaponData.MaxAmmo;
            inputActions = new PlayerInptActions();
            inputActions.Player.Fire.performed += Fire;
            inputActions.Player.Fire.canceled += Fire;
            inputActions.Player.Reload.performed += Reload;

            if (UnityEngine.Camera.main != null)
            {
                mainCameraTra = UnityEngine.Camera.main.transform;
            }
            else
            {
                Debug.LogError("カメラがないよ");
            }
            gameObject.SetActive(true);
        }
        private void OnEnable()
        {
            inputActions?.Enable();
        }
        private void OnDisable()
        {
            inputActions?.Disable();
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
            if (RB == null || mainCameraTra == null)
            {
                Debug.LogError("リジットボディがついていないよ");
                return;
            }

            Vector3 cameraForward = mainCameraTra.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            if (cameraForward != Vector3.zero)
            {
                Quaternion targetrotation = Quaternion.LookRotation(cameraForward);
                RB.rotation = Quaternion.Slerp(RB.rotation, targetrotation, rotateSpeed * Time.deltaTime);
            }

            if (moveInput == Vector2.zero)
            {
                RB.linearVelocity = new Vector3(0f, RB.linearVelocity.y, 0f);
                CurrentVelocity = Vector3.zero;
                return;
            }

            Vector3 cameraRight = mainCameraTra.right;

            cameraRight.y = 0f;
            cameraRight.Normalize();

            Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

            Vector3 targetVelocity = moveDirection * moveSpeed;
            RB.linearVelocity = new Vector3(targetVelocity.x, RB.linearVelocity.y, targetVelocity.z);

            CurrentVelocity = RB.linearVelocity;
        }

        private void Fire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (!canShot || isReloding || WeaponData == null)
                {
                    return;
                }
                fireCT = new CancellationTokenSource();
                var linkedCT = CancellationTokenSource.CreateLinkedTokenSource(fireCT.Token, this.GetCancellationTokenOnDestroy());
                switch ((FireType)WeaponData.WeponType)
                {
                    case FireType.SemiAuto:
                        ShotSemiAutoAsync(this.GetCancellationTokenOnDestroy()).Forget();
                        break;
                    case FireType.Burst:
                        ShotBurstAsync(this.GetCancellationTokenOnDestroy()).Forget();
                        break;
                    case FireType.FullAuto:
                        ShotFullAutoAsync(linkedCT.Token).Forget();
                        break;
                    default:
                        Debug.LogError("未割り当ての射撃タイプ");
                        break;
                }
            }
            if (context.canceled)
            {
                fireCT?.Cancel();
                //fireCT?.Dispose();  //どっちでもいい
                fireCT = null;
            }
        }
        private async UniTaskVoid ShotSemiAutoAsync(CancellationToken token)
        {
            if (CurrntAmmo == 0)
            {
                reload().Forget();
                return;
            }
            canShot = false;

            CurrntAmmo -= 1;
            Debug.Log(CurrntAmmo);
            Shoot();
            await UniTask.Delay(TimeSpan.FromSeconds(WeaponData.FireRate), cancellationToken: token);
            canShot = true;
        }
        private async UniTaskVoid ShotBurstAsync(CancellationToken token)
        {
            if (CurrntAmmo == 0)
            {
                reload().Forget();
                return;
            }
            canShot = false;
            for (int i = 0; i < 3; i++)
            {
                if (CurrntAmmo <= 0)
                {
                    canShot = true;
                    return;
                }
                CurrntAmmo -= 1;
                Shoot();
                await UniTask.Delay(TimeSpan.FromSeconds(WeaponData.FireInterval), cancellationToken: token);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(WeaponData.FireRate), cancellationToken: token);
            canShot = true;
        }
        private async UniTaskVoid ShotFullAutoAsync(CancellationToken token)
        {
            if (CurrntAmmo == 0)
            {
                reload().Forget();
                return;
            }
            canShot = false;
            while (!token.IsCancellationRequested)
            {
                if (CurrntAmmo <= 0)
                {
                    reload().Forget();
                    break;
                }
                CurrntAmmo -= 1;
                Shoot();
                bool isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(WeaponData.FireInterval), cancellationToken: token).SuppressCancellationThrow();
                if (isCanceled == true)
                {
                    break;
                }
            }
            Debug.Log(token == null);
            await UniTask.Delay(TimeSpan.FromSeconds(WeaponData.FireRate), cancellationToken: this.GetCancellationTokenOnDestroy());
            canShot = true;
        }

        private void Shoot()
        {
            if(muzzleFlash != null)
            {
                muzzleFlash.Play();
            }
            Ray ray = new Ray(mainCameraTra.position, mainCameraTra.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, ATTACK_RANGE))
            {
                Debug.Log("hit");
                IDamageable target = hitInfo.collider.GetComponent<IDamageable>();
                if (target != null)
                {
                    Debug.Log("teki");
                    target.TekeDamage(WeaponData.AttackPower);
                }
            }
        }
        private void Reload(InputAction.CallbackContext context)
        {
            if (isReloding == false || CurrntAmmo != WeaponData.MaxAmmo) reload().Forget();
        }
        private async UniTask reload()
        {
            isReloding = true;
            Debug.Log("rode");
            await UniTask.Delay(TimeSpan.FromSeconds(WeaponData.ReloadTime), cancellationToken: this.GetCancellationTokenOnDestroy());

            CurrntAmmo = WeaponData.MaxAmmo;
            isReloding = false;
            Debug.Log("crea");
        }
        private void DrawLaserPointer()
        {
            if (laserLineRenderer == null || weponOrigin == null || mainCameraTra == null)
            {
                return;
            }
            laserLineRenderer.SetPosition(0, weponOrigin.position);

            Ray ray = new Ray(mainCameraTra.position, mainCameraTra.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, laserMaxDistance))
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