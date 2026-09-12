using UnityEngine;

namespace Core.Manager
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [SerializeField] AudioSource bgm;
        [SerializeField] AudioSource se;

        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float bgmVolume = 1f;
        [Range(0f, 1f)] public float seVolume = 1f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        public void PlayBGM(AudioClip clip)
        {
            if(clip != null)
            {
                bgm.clip = clip;
                bgm.volume = masterVolume * bgmVolume;
                bgm.Play();
            }
        }
        public void UpdateVolume()
        {
            bgm.volume = masterVolume * bgm.volume;
            seVolume = masterVolume * seVolume;
        }
    }
}
