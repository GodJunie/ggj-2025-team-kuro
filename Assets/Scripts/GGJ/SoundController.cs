using UnityEngine;

namespace GGJ
{
    public class SoundController : MonoBehaviour
    {
        [SerializeField] AudioSource bgmAudioSource;
        //[SerializeField] AudioSource sfxAudioSource;
        [SerializeField] AudioClip bgm;
        //[SerializeField] AudioClip biteClip;
        //[SerializeField] AudioClip popClip;

        private void Start()
        {
            bgmAudioSource.clip = bgm;
            bgmAudioSource.playOnAwake = true;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
        }

        // public void PlayPopEffect()
        // {
        //     sfxAudioSource.PlayOneShot(popClip);
        // }

        // public void PlayBiteEffect()
        // {
        //     sfxAudioSource.PlayOneShot(biteClip);
        // }
    }

}

