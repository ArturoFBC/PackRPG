using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MusicTrackType
{
    MAIN_MENU,
    LAIR,
    SWAMP
}

[System.Serializable]
public struct MusicTrack
{
    public AudioClip intro;
    public AudioClip loop;
}

public class MusicManager : MonoBehaviour
{
    private static MusicManager _ref;
    public static MusicManager _Ref
    {
        get
        {
            if (_ref == null)
            {
                _ref = FindObjectOfType<MusicManager>();

                if (_ref == null)
                    _ref = new GameObject("InstantiatedMusicManager", typeof(MusicManager)).GetComponent<MusicManager>();
            }

            return _ref;
        }
        private set
        {
            _ref = value;
        }
    }

    private AudioSource musicSourceA;
    private AudioSource musicSourceB;

    private bool is_A_TheActiveMusicSource;

    [SerializeField]
    private List<MusicTrack> musicTracks = new List<MusicTrack>();
    [SerializeField]
    private MusicTrackType currentTrack;

    /// <summary>
    /// This is used to store the coroutine everytime a coroutine to play with intro is called, so that the old one can be cancelled when calling a new one to avoid them overlapping.
    /// </summary>
    private Coroutine playWithIntroCoroutineHandler;

    private void Awake()
    {
        if (_Ref != null && _Ref != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        musicSourceA = gameObject.AddComponent<AudioSource>();
        musicSourceB = gameObject.AddComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Menu"))
            StartCrossFade(MusicTrackType.MAIN_MENU);
        else if (scene.name.Contains("Lair"))
            StartCrossFade(MusicTrackType.LAIR);
        else if (scene.name.Contains("Choose"))
            StartCrossFade(MusicTrackType.LAIR);
        else
            StartCrossFade(MusicTrackType.SWAMP);
    }

    public void StartMusic(MusicTrackType track)
    {
        AudioSource currentSource = (is_A_TheActiveMusicSource ? musicSourceA : musicSourceB);

        if (currentSource.isPlaying && track == currentTrack)
            return;

        currentSource.volume = 1;
        playWithIntroCoroutineHandler = StartCoroutine(PlayTrack(currentSource, musicTracks[(int)track]));

        currentTrack = track;
    }

    public void StartFade(MusicTrackType track, float transitionTime = 2f)
    {
        AudioSource currentSource = (is_A_TheActiveMusicSource ? musicSourceA : musicSourceB);

        if (currentSource.isPlaying && track == currentTrack)
            return;

        StartCoroutine(StartMusicFade(currentSource, track, transitionTime));

        currentTrack = track;
    }

    public void StartCrossFade(MusicTrackType track, float transitionTime = 2f)
    {
        AudioSource currentSource = (is_A_TheActiveMusicSource ? musicSourceA : musicSourceB);

        if (currentSource.isPlaying && track == currentTrack)
            return;

        AudioSource nextSource = (is_A_TheActiveMusicSource ? musicSourceB : musicSourceA);

        is_A_TheActiveMusicSource = !is_A_TheActiveMusicSource;

        nextSource.Stop();
        nextSource.volume = 1;
        playWithIntroCoroutineHandler = StartCoroutine(PlayTrack(nextSource, musicTracks[(int)track]));
        StartCoroutine(StartMusicCrossFade(currentSource, nextSource, transitionTime));

        currentTrack = track;
    }

    private IEnumerator StartMusicFade(AudioSource source, MusicTrackType track, float transitionTime)
    {
        for (float transition = 0; transition < transitionTime; transition += Time.unscaledDeltaTime)
        {
            source.volume = (1 - (transition / transitionTime));
            yield return null;
        }

        source.volume = 0;
        source.Stop();
        playWithIntroCoroutineHandler = StartCoroutine(PlayTrack(source, musicTracks[(int)track]));

        for (float transition = 0; transition < transitionTime; transition += Time.unscaledDeltaTime)
        {
            source.volume = transition / transitionTime;
            yield return null;
        }
        source.volume = 1;
    }

    private IEnumerator StartMusicCrossFade(AudioSource current,AudioSource next, float transitionTime)
    {
        for (float transition = 0; transition < transitionTime; transition += Time.unscaledDeltaTime)
        {
            current.volume = (1 - (transition / transitionTime));
            next.volume = transition / transitionTime;
            yield return null;
        }
        next.volume = 1;

        current.Stop();
    }

    private IEnumerator PlayTrack( AudioSource source, MusicTrack track)
    {
        if (playWithIntroCoroutineHandler != null)
            StopCoroutine(playWithIntroCoroutineHandler);

        if (track.intro != null)
        {
            source.loop = false;
            source.clip = track.intro;
            source.Play();
            yield return new WaitForSecondsRealtime(track.intro.length);
        }
        source.loop = true;
        source.clip = track.loop;
        source.Play();
    }
}
