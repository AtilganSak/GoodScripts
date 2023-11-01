using Cinemachine;
using Highborn;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    EarnCoin,
    EarnStar,
    ToolbarSwitchRight,
    ToolbarSwitchLeft,
    PopupOpen,
    PopupClose,
    LevelComplete,
    SwitchOn,
    SwitchOff,
    CorrectWord,
    LetterPlaced,
    AddedExtraWord,
    OpenedFurniture,
    CorrectAnswer,
    FoundExtraWord
}
public class SoundManager : MonoBehaviourSingletonPersistent<SoundManager>
{
    [System.Serializable]
    public struct Snapshot
    {
        public float transition;
        public AudioMixerSnapshot snap;
    }

    [SerializeField] AudioMixer gameMixer;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource gameSceneMusicSource;
    [SerializeField] AudioSource mainSceneMusicSource;
    
    [FoldoutGroup("Snapshots")]
    [SerializeField] Snapshot gameSceneMusicSnap;
    [FoldoutGroup("Snapshots")]
    [SerializeField] Snapshot mainSceneMusicSnap;    

    [SerializeField] AudioClip[] gameSceneMusics;
    [SerializeField] AudioClip[] mainSceneMusics;
    [SerializeField] SoundItem[] soundItems;

    const string MASTER_VOL = "MasterVol";
    const string SFX_VOL = "SFXVol";
    const string MUSIC_VOL = "MusicVol";

    float defaultGameSceneMusicSourceVol;
    float defaultMainSceneMusicSourceVol;
    
    private void OnEnable()
    {        
        defaultGameSceneMusicSourceVol = gameSceneMusicSource.volume;
        defaultMainSceneMusicSourceVol = mainSceneMusicSource.volume;

        SceneController sc = FindAnyObjectByType<SceneController>();
        if (sc != null)
        {
            sc.onLoadedGameScene += LoadedGameScene;
            sc.onLoadedMainScene += LoadedMainScene;
        }
    }
    private void LoadedMainScene()
    {        
        SwitchToMainSceneMusic();
    }
    private void LoadedGameScene()
    {        
        SwitchToGameSceneMusic();   
    }        
    public void MuteSFX()
    {
        sfxSource.volume = 0;        
    }
    public void UnMuteSFX()
    {
        sfxSource.volume = 1;        
    }
    public void MuteMusic()
    {
        gameSceneMusicSource.volume = 0;
        mainSceneMusicSource.volume = 0;        
    }
    public void UnMuteMusic()
    {
        gameSceneMusicSource.volume = defaultGameSceneMusicSourceVol;
        mainSceneMusicSource.volume = defaultMainSceneMusicSourceVol;        
    }
    public void PlaySoundSFX(SoundType sound)
    {
        for (int i = 0; i < soundItems.Length; i++)
        {
            if (soundItems[i].type == sound)
            {
                sfxSource.PlayOneShot(soundItems[i].audioClip);                
            }
        }
    }
    [Button]
    public void SwitchToGameSceneMusic()
    {        
        if (gameSceneMusics.Length > 0)
        {
            gameSceneMusicSource.clip = gameSceneMusics.GetRandom();
            gameSceneMusicSource.Play();
        }
        gameSceneMusicSnap.snap.TransitionTo(gameSceneMusicSnap.transition);
    }
    [Button]
    public void SwitchToMainSceneMusic()
    {        
        if (mainSceneMusics.Length > 0)
        {
            mainSceneMusicSource.clip = mainSceneMusics.GetRandom();
            mainSceneMusicSource.Play();
        }

        mainSceneMusicSnap.snap.TransitionTo(mainSceneMusicSnap.transition);
    }
    public AudioClip GetClipBySoundType(SoundType soundType)
    {
        if (soundItems != null)
        {
            for (int i = 0; i < soundItems.Length; i++)
            {
                if (soundItems[i].type == soundType)
                {
                    return soundItems[i].audioClip;
                }
            }
        }
        return null;
    }
    [System.Serializable]
    public struct SoundItem
    {
        public SoundType type;
        public AudioClip audioClip;
    }
}
