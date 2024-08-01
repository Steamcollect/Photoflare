using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Music References")]
    [SerializeField] Playlist[] playlists;

    [Header("Audio References")]
    [SerializeField] AudioMixer audioMixer;
    [Space(5)]
    [SerializeField] AudioMixerGroup musicMixerGroup;
    [SerializeField] AudioMixerGroup soundMixerGroup;
        
    Queue<AudioSource> soundsGo = new Queue<AudioSource>();

    [Header("System References")]
    [SerializeField, Tooltip("Number of GameObject create on start for the sound")] int startingAudioObjectsCount = 30;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Debug.LogError($"The is more than 1 {this} in the scene");
    }

    private void Start()
    {
        // Set Audio Source for Musics
        for (int i = 0; i < playlists.Length; i++)
        {
            AudioSource audioSource = (gameObject.AddComponent<AudioSource>());
            playlists[i].audioSource = audioSource;
            audioSource.volume = playlists[i].volumMultiplier;

            StartCoroutine(SetAudioSourceClip(playlists[i], playlists[i].maxLoop));
        }

        // Create Audio Object
        for (int i = 0; i < startingAudioObjectsCount; i++)
        {
            soundsGo.Enqueue(CreateSoundsGO());
        }
    }

    IEnumerator SetAudioSourceClip(Playlist playlist, int maxLoop)
    {
        // Set clip
        playlist.audioSource.clip = playlist.clips[playlist.currentClipIndex];
        playlist.audioSource.Play();

        yield return new WaitForSeconds(playlist.clips[playlist.currentClipIndex].length);

        // Set clip index
        playlist.currentClipIndex = (playlist.currentClipIndex + 1) % playlist.clips.Length;

        maxLoop -= 1;
        if(maxLoop > 0 || maxLoop == -1)
        {
            StartCoroutine(SetAudioSourceClip(playlist, maxLoop));
        }

        // End the loop
    }

    /// <summary>
    /// Require the clip and the power of the sound
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="soundPower"></param>
    public void PlayClipAt(AudioClip clip, float volumMultiplier = 1)
    {
        AudioSource tmpAudioSource;
        if (soundsGo.Count <= 0) tmpAudioSource = CreateSoundsGO();
        else tmpAudioSource = soundsGo.Dequeue();

        // Set the volum
        volumMultiplier = Mathf.Clamp(volumMultiplier, 0, 1);
        tmpAudioSource.volume = volumMultiplier;

        // Set the clip
        tmpAudioSource.clip = clip;
        tmpAudioSource.Play();
        StartCoroutine(AddAudioSourceToQueue(tmpAudioSource));
    }
    IEnumerator AddAudioSourceToQueue(AudioSource current)
    {
        yield return new WaitForSeconds(current.clip.length);
        soundsGo.Enqueue(current);
    }

    AudioSource CreateSoundsGO()
    {
        AudioSource tmpAudioSource = new GameObject("Audio Go").AddComponent<AudioSource>();
        tmpAudioSource.transform.SetParent(transform);
        tmpAudioSource.outputAudioMixerGroup = soundMixerGroup;
        soundsGo.Enqueue(tmpAudioSource);

        return tmpAudioSource;
    }

    [System.Serializable]
    public class Playlist
    {
        [Range(0,1)]public float volumMultiplier = 1;
        [Tooltip("If value equal \"-1\" so infinite loop")] public int maxLoop = -1;
        [Space(10)]
        public AudioClip[] clips;

        [HideInInspector] public AudioSource audioSource;
        [HideInInspector] public int currentClipIndex = 0;

        /// <summary>
        /// Require Clips to play and the volum multiplier
        /// </summary>
        /// <param name="clips"></param>
        /// <param name="volumMultiplier"></param>
        public Playlist(AudioClip[] clips, float volumMultiplier, int maxLoop)
        {
            this.clips = clips;
            this.volumMultiplier = volumMultiplier;
            this.maxLoop = maxLoop;
        }
    }
}