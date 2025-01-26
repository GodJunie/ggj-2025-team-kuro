using UnityEngine;

namespace GGJ
{
    public class SoundController : MonoBehaviour
    {
        AudioSource audioSource;
        [SerializeField] AudioClip BGM;
        [SerializeField] AudioClip biteClip;
        [SerializeField] AudioClip popClip;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayPopEffect()
        {
            audioSource.PlayOneShot(popClip);
        }

        public void PlayBiteEffect()
        {
            audioSource.PlayOneShot(biteClip);
        }
    }

}

