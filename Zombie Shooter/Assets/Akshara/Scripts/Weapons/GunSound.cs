using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AksharaMurda
{
    public class GunSound : MonoBehaviour
    {
        [Tooltip("Sound to play when ejecting a clip.")]
        public AudioClip[] Eject;

        [Tooltip("Sound to play when a clip is put inside the gun.")]
        public AudioClip[] Rechamber;

        [Tooltip("Possible sounds to play on each bullet fire.")]
        public AudioClip[] Fire;

        public void OnReloadStart()
        {
            StartCoroutine(reloadSequence());
        }

        public void OnFire(float delay)
        {
            StartCoroutine(play(delay, Fire));
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator reloadSequence()
        {
            yield return play(0.1f, Eject);
            yield return play(0.6f, Rechamber);
        }

        private void play(AudioClip clip)
        {
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, transform.position);
        }

        private void play(AudioClip[] clips)
        {
            if (clips.Length > 0)
                play(clips[Random.Range(0, clips.Length)]);
        }

        private IEnumerator play(float delay, AudioClip[] clips)
        {
            yield return new WaitForSeconds(delay);
            play(clips);
        }
    }
}
