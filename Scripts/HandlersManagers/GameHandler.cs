using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;
    private void Awake()
    {
        instance = this;
    }

    [Header("Audio")]
    public AudioSource EffectAudio;
    public AudioSource LoopEffectAudio;

    [Header("Particles")]
    public ParticleSystem BananaParticle;
    public ParticleSystem CustomParticle;

    [Header("Misc")]
    public PauseMenu PauseMenu;
    public Transform UnstuckPosition;
    public static System.Type abilityToUnlockOnLoad;

    public bool Temporary = false;
    public static CarController_CW Player { get; private set; }
    public static bool TrialActive { get; set; } = false;
    private void Start()
    {
        StartCoroutine(ActivateNew());
        if (LoopEffectAudio == null) LoopEffectAudio = gameObject.AddComponent<AudioSource>();

        if (!Temporary)
        {
            Player = (CarController_CW)GameObject.FindAnyObjectByType(typeof(CarController_CW));
            PauseMenu.SetPlayerCollectables(Player.GetComponentInChildren<PlayerCollectables>());
        }

        if (!MainMenu.NewGame && !Temporary)
        {
            MainMenu.NewGame = true;
            Load();
        }
        else if(MainMenu.NewGame)
        {
            SceneManager.activeSceneChanged -= OnSceneLoaded;
        }
       
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Notice the return here
            if (PauseMenu == null) return;
            if (!CustomizationScript.CanClose) return;

            if (Player != null)
            {
                Player.HideAbilityUi();
            }

            if (PauseMenu.MenuOpen())
            {
                PauseMenu.CloseMenu();
            }
            else
            {
                PauseMenu.OpenMenu();
            }
        }
    }

    public void PlayEffectAudio(AudioClipPreset audioToPlay) //Plays a audio on call
    {
        if (EffectAudio != null)
        {
            EffectAudio.pitch = audioToPlay.Pitch;
            EffectAudio.volume = audioToPlay.Volume;
            EffectAudio.PlayOneShot(audioToPlay.Clip);
        }
        else Debug.LogError("No Audio Set global");
    }
    public void PlayLoopEffectAudio(AudioClipPreset audio)
    {
        LoopEffectAudio.clip = audio.Clip;
        LoopEffectAudio.pitch = audio.Pitch;
        LoopEffectAudio.volume = audio.Volume;
        LoopEffectAudio.loop = true;
        LoopEffectAudio.Play();
    }
    public void StopLoop()
    {
        LoopEffectAudio.loop = false;
    }

    public void PlayBananaParticle(Transform spot, Color customColor = default) //Plays the particle effect at the given spot
    {
        customColor = customColor == default ? Color.yellow : customColor;

        var main = BananaParticle.main;
        main.startColor = customColor;

        BananaParticle.transform.position = spot.position;
        BananaParticle.Play();
    }
    public void PlayCustomParticle(ParticleSettings setting, Vector3 spot)
    {
        CustomParticle.transform.position = spot;

        var t = CustomParticle.main;
        t.startColor = setting.CustomColor;
        t.startLifetime = setting.ParticleLifeTime;

        var em = CustomParticle.emission;
        em.rateOverTimeMultiplier = setting.EmissionRate;

        CustomParticle.Play();
    }
    
    public static void Save(int saveNum, Transform player, PlayerCollectables collected, PlayerUnlockables unlocked) //Easy way to call and save the game
    {
        if(!TrialActive) //Dont allow saving during trial
        {
            SavingHandler.Save(saveNum, player.position, player.eulerAngles.y, collected.CollectedIds, unlocked.UnlockableIds, unlocked.AbilityIds, CustomizationScript.instance.Pieces);
            TextTriggerManager.instance.Save();
        }
    }
    public static void Load() //Loads up the save
    {
        //First load the current scene again
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

        SceneManager.activeSceneChanged -= OnSceneLoaded;
        SceneManager.activeSceneChanged += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene previousScene, Scene newScene)
    {
        // Load saved data
        var data = SavingHandler.Load(MainMenu.CurrentSave);
        if (data == null) return;

        // Get the new player in the new scene
        CarController_CW newPlayer = (CarController_CW)GameObject.FindAnyObjectByType(typeof(CarController_CW));
        newPlayer.Allow = false;
        Player = newPlayer;

        // Load the collectables data
        newPlayer.GetComponentInChildren<PlayerCollectables>().LoadData(data.Collectables);
        newPlayer.Allow = true;

        //Load unlockables
        newPlayer.GetComponentInChildren<PlayerUnlockables>().LoadData(data.Unlockables);
        
        // Move the player to the saved spot
        newPlayer.transform.position = new Vector3(data.PlayerLocation.PlayerLocation[0], data.PlayerLocation.PlayerLocation[1], data.PlayerLocation.PlayerLocation[2]);
        newPlayer.transform.eulerAngles = new Vector3(newPlayer.transform.eulerAngles.x, data.PlayerLocation.RotY, newPlayer.transform.eulerAngles.z);

        //Remove read things
        foreach (var item in FindObjectsByType<TextTrigger>(FindObjectsSortMode.None))
        {
            item.Load();
        }
    }

    public static IEnumerator ActivateNew() //This is for scene transition to unlock the gotten ability
    {
        yield return new WaitForSeconds(1);

        if (abilityToUnlockOnLoad != null)
        {
            var t = UnlockablesHandler.instance.GetAbility(abilityToUnlockOnLoad);
            Player.GetComponentInChildren<PlayerUnlockables>().AddAbility(t);
            GameHandler.instance.PauseMenu.Save();
            abilityToUnlockOnLoad = null;
        }
    }

    public void Unstuck()
    {
        Player.transform.SetPositionAndRotation(UnstuckPosition.position, UnstuckPosition.rotation);
    }
}

[System.Serializable]
public class ParticleSettings
{
    public Color CustomColor = default;
    public float ParticleLifeTime = 1;
    public float EmissionRate;
}
