using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio 
{
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioSource source;


        private SoundPlaySettings settings;
        private SoundGroup group;
        private AudioClip clip;

        public bool Finished {get; private set;} = true;
        public bool isPlayingNow => isPlaying;

        private Coroutine SoundWatcher;
        private int iterationIndex = 0;
        
        private bool isPlaying = false;

        private float currentVolume;

        public void Init(AudioClip clip, SoundPlaySettings settings, SoundGroup group) 
        {
            source.Stop();
            Finished = false;
            isPlaying = false;
            source.playOnAwake = false;
            iterationIndex = 0;
            this.clip = clip;
            this.settings = settings;
            this.group = group;
            source.clip = clip;
            currentVolume = settings.volume;
            SetVolume(group.Volume);
            source.pitch = settings.GetPitch();
            source.loop = settings.repeat == SoundRepeatType.Loop;
            group.ValueChangeEvent.AddListener(SetVolume);
        }

        private void Update() 
        {
            if (source.loop)
                return;
            if (isPlaying && !source.isPlaying) 
            {
                if (settings.repeat == SoundRepeatType.Once) 
                {
                    Stop();
                    return;
                }
                iterationIndex++;
                if (iterationIndex >= settings.numberOfRepeats) 
                {
                    Stop();
                    return;
                }
                else 
                {
                    source.Stop();
                    source.Play();
                }
            }    
        }

        public void Stop() 
        {
            group.ValueChangeEvent.RemoveListener(SetVolume);
            isPlaying = false;
            Finished = true;
            source.Stop();
            source.pitch = settings.GetPitch();
            this.Despawn();
        }

        public void ChangeVolume(float newVolume) 
        {
            currentVolume = newVolume;
            SetVolume(group.Volume);
        }

        private void SetVolume(float groupVolume) 
        {
            source.volume = currentVolume * groupVolume;
        }

        public void Play() 
        {
            if (Finished)
                return;
            source.Play();
            isPlaying = true;
        }

        public void Pause() 
        {
            if (Finished)
                return;
            source.Pause();
            isPlaying = false;
        }
    }
}
