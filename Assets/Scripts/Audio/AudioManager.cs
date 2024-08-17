using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Music References")]
    [SerializeField] float transitionTime;
    List<Playlist> playlists = new List<Playlist>();

    [Header("Audio References")]
    [SerializeField] AudioMixer audioMixer;
    [Space(5)]
    [SerializeField] AudioMixerGroup musicMixerGroup;
    [SerializeField] AudioMixerGroup soundMixerGroup;
        
    Queue<AudioSource> soundsGo = new Queue<AudioSource>();

    [Header("System References")]
    [SerializeField, Tooltip("Number of GameObject create on start for the sound")] int startingAudioObjectsCount = 30;

    Transform musicsGoParent, soundsGoParent;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Debug.LogError($"The is more than 1 {this} in the scene");
    }

    private void Start()
    {
        // Create Audio Object
        SetSoundsGO();
    }

    #region Music
    public void SetMusicsGO(Playlist[] newPlaylists)
    {
        // Setup parent
        if(musicsGoParent == null)
        {
            musicsGoParent = new GameObject("======MUSICS GO======").transform;
            musicsGoParent.SetParent(transform);
        }

        // Change playlist if its different
        if(playlists != newPlaylists.ToList()) StartCoroutine(SetNewPlaylists(newPlaylists));
    }

    IEnumerator SetNewPlaylists(Playlist[] newPlaylists)
    {
        // Stop current playlists
        for (int i = 0; i < playlists.Count; i++)
        {
            playlists[i].FadeOut(transitionTime / 2);
        }
        yield return new WaitForSeconds(transitionTime / 2);

        // Clear current playlists
        for (int i = 0; i < playlists.Count; i++)
        {
            Destroy(playlists[i].audioSource.gameObject);
        }
        playlists.Clear();

        // Setup new playlists
        for (int i = 0; i < newPlaylists.Length; i++)
        {
            playlists.Add(CreatePlaylistGO(newPlaylists[i]));
            playlists[i].timerCoroutine = StartCoroutine(SetAudioSourceClip(playlists[i], playlists[i].maxLoop));
            playlists[i].FadeIn(playlists[i].volum, transitionTime / 2);
        }
    }

    Playlist CreatePlaylistGO(Playlist playlist)
    {
        // Create GameObject
        playlist.audioSource = new GameObject("Music GO").AddComponent<AudioSource>();
        playlist.audioSource.transform.SetParent(musicsGoParent);

        // Set Audio source references
        playlist.audioSource.outputAudioMixerGroup = musicMixerGroup;
        playlist.audioSource.volume = 0;

        return playlist;
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
        playlist.timerCoroutine = null;
    }

    [System.Serializable]
    public class Playlist
    {
        [Range(0,1)]public float volum = 1;
        [Tooltip("If value equal \"-1\" so infinite loop")] public int maxLoop = -1;
        [Space(10)]
        public AudioClip[] clips;

        [HideInInspector] public AudioSource audioSource;
        [HideInInspector] public int currentClipIndex = 0;

        [HideInInspector] public Coroutine timerCoroutine;

        public void FadeOut(float time)
        {
            audioSource.DOKill();
            audioSource.DOFade(0, time).OnComplete(()=>
            {
                audioSource.gameObject.SetActive(false);
            });
        }
        public void FadeIn(float value, float time)
        {
            audioSource.gameObject.SetActive(true);

            audioSource.DOKill();
            audioSource.DOFade(value, time);
        }

        public Playlist(AudioClip[] clips, float volumMultiplier, int maxLoop)
        {
            this.clips = clips;
            this.volum = volumMultiplier;
            this.maxLoop = maxLoop;
        }
    }
    #endregion

    #region Sound
    void SetSoundsGO()
    {
        soundsGoParent = new GameObject("======SOUND GO======").transform;
        soundsGoParent.SetParent(transform);
        
        for (int i = 0; i < startingAudioObjectsCount; i++)
        {
            AudioSource current = CreateSoundsGO();
            current.gameObject.SetActive(false);
            soundsGo.Enqueue(current);
        }
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

        tmpAudioSource.gameObject.SetActive(true);

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
        current.gameObject.SetActive(false);
        soundsGo.Enqueue(current);
    }

    AudioSource CreateSoundsGO()
    {
        AudioSource tmpAudioSource = new GameObject("Sound GO").AddComponent<AudioSource>();
        tmpAudioSource.transform.SetParent(soundsGoParent);
        tmpAudioSource.outputAudioMixerGroup = soundMixerGroup;
        soundsGo.Enqueue(tmpAudioSource);

        return tmpAudioSource;
    }
    #endregion
}