using jugyou.batoru.Player;
using UnityEngine;

namespace jugyou.batoru.Item
{
    public class ExperienceOrb : MonoBehaviour
    {
        private const float magnetRange = 5;
        private const float magnetSpeed = 15;
        private const string playerTag = "Player";

        private Transform playerTarget;
        private bool isFollowing = false;

        private void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                playerTarget = playerObj.transform;
            }
            else
            {
                Debug.LogError("ÉvÉåÉCÉÑÅ[Ç»Ç¢");
            }
        }
        private void Update()
        {
            if (playerTarget == null) return;
            if (isFollowing)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, magnetSpeed * Time.deltaTime);
            }
            else
            {
                float distToPlayer = Vector3.Distance(transform.position, playerTarget.position);
                if(distToPlayer <= magnetRange)
                {
                    isFollowing = true;
                }
            }

        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag(playerTag))
            {
                PlayerController plyaer = other.GetComponent<PlayerController>();
                if(plyaer != null)
                {
                    plyaer.addExp(1);
                }
                else
                {
                    Debug.Log("Ç¢Ç»Ç¢");
                }
                Destroy(this.gameObject);
            }
        }

    }
}