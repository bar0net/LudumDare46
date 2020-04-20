using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour
{
    [System.Serializable]
    public enum MusicTypes { Rock = 0, Disco = 1, Funk = 2 };

    public Animator menu_animator;
    public bool inTransition = false; // The animation sets it to false on transition complete.

    public GameObject[] help_panels;
    public GameObject help_screen;


    public AudioClip[] clips;
    public AudioSource audio_source;

    [SerializeField]
    MusicTypes current_type = MusicTypes.Disco;
    Manager _manager;
    Music _music;

    bool ready = false;
    Button[] action_buttons;

    private void Start()
    {
        _manager = GameObject.FindObjectOfType<Manager>();
        _manager.SetColors((float)current_type / 3F);
        _music = FindObjectOfType<Music>();

        action_buttons = GetComponentsInChildren<Button>();
        foreach (Button button in action_buttons) button.interactable = false;
        {

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) _manager.SelectPrevious();
        else if (Input.GetKeyDown(KeyCode.RightArrow)) _manager.SelectNext();

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.A)) Q_Action();
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Z)) W_Action();
        else if (Input.GetKeyDown(KeyCode.E)) E_Action();
        else if (Input.GetKeyDown(KeyCode.R)) R_Action();
        else if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) Left((int)current_type);
        else if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4)) Right((int)current_type);

        if (Input.GetKeyDown(KeyCode.H))
        {
            help_screen.SetActive(!help_screen.activeSelf);
            if (help_screen.activeSelf) Time.timeScale = 0.5f;
            else Time.timeScale = 1.0F;
        }

        if (ready != _manager.PlayerReady())
        {
            ready = _manager.PlayerReady();
            foreach (Button button in action_buttons) button.interactable = ready;
        }
    }


    public void Left(int myType)
    {
        if (!_manager.PlayerReady()) return;
        if ((MusicTypes)myType != current_type || inTransition) return;
        
        menu_animator.SetTrigger("Left");
        inTransition = true;

        int new_type = myType + 1;
        current_type = new_type > 2 ? (MusicTypes)new_type - 3 : (MusicTypes)new_type;
        _manager.SetColors((float)current_type / 3F);
        _manager.ResetActionTimer();

        for (int i = 0; i < 3; i++) help_panels[i].SetActive(i == (int)current_type);
        _music.Fade((int)current_type);
    }

    public void Right(int myType)
    {
        if (!_manager.PlayerReady()) return;
        if ((MusicTypes)myType != current_type || inTransition) return;

        menu_animator.SetTrigger("Right");
        inTransition = true;

        int new_type = myType - 1;
        current_type = new_type < 0 ? (MusicTypes)new_type + 3 : (MusicTypes)new_type;
        _manager.SetColors((float)current_type / 3F);
        _manager.ResetActionTimer();

        for (int i = 0; i < 3; i++) help_panels[i].SetActive(i == (int)current_type);
        _music.Fade((int)current_type);
    }

    public MusicTypes CurrentType()
    {
        return current_type;
    }

    // ROCK ABILITIES
    public void RockBasic()
    {
        if (!_manager.PlayerReady() || current_type != MusicTypes.Rock) return;
        if (_manager.SelectedEnemy() != null) _manager.SelectedEnemy().Hype();


        _manager.ResetActionTimer();
        PlayComment();
    }

    public void RockFirst()
    {
        if (!_manager.PlayerReady() || current_type != MusicTypes.Rock) return;
        if (_manager.current_mana < 0.25F * _manager.max_mana) return;
        if (_manager.SelectedEnemy() == null) return;
        if (_manager.SelectedEnemy().type == MusicTypes.Rock) return;

        _manager.SelectedEnemy().next_skill = new Tantrum();

        _manager.RemoveMana(0.25F * _manager.max_mana);
        _manager.ResetActionTimer();
        PlayComment();
    }

    public void RockSecond()
    {
        if (!_manager.PlayerReady() || current_type != MusicTypes.Rock) return;
        if (_manager.current_mana < 0.5F * _manager.max_mana) return;
        if (_manager.SelectedEnemy() == null) return;
        if (_manager.SelectedEnemy().type == MusicTypes.Rock) return;

        _manager.SelectedEnemy().stunned = 2;


        _manager.RemoveMana(0.5F * _manager.max_mana);
        _manager.ResetActionTimer();
        PlayComment();
    }

    public void RockUltimate()
    {
        if (!_manager.PlayerReady() || current_type != MusicTypes.Rock) return;
        if (_manager.current_mana < _manager.max_mana) return;

        foreach (Enemy e in _manager.enemies_in_arena)
        {
            if (e == null) continue;
            if (e.type == MusicTypes.Rock) continue;
            e.stunned += 1;
        }


        _manager.RemoveAllMana();
        _manager.ResetActionTimer();
        PlayComment();
    }


    // DISCO ABILITIES
    public void DiscoBasic()
    {
        if (!_manager.PlayerReady() || current_type != MusicTypes.Disco) return;

        if (_manager.SelectedEnemy() != null)  _manager.SelectedEnemy().Hype();


        _manager.ResetActionTimer();
        PlayComment();
    }

    public void DiscoFirst()
    {
        if (!_manager.PlayerReady() || current_type != MusicTypes.Disco) return;
        if (_manager.current_mana < 0.25F * _manager.max_mana) return;

        if (_manager.SelectedEnemy() == null) return;
        if (_manager.SelectedEnemy().type == MusicTypes.Disco) return;

        _manager.SelectedEnemy().extra_hype_timer = 1;

        _manager.RemoveMana(0.25F * _manager.max_mana);
        _manager.ResetActionTimer();
        PlayComment();
    }

    public void DiscoSecond()
    {
        if (!_manager.PlayerReady() || current_type != MusicTypes.Disco) return;
        if (_manager.current_mana < 0.5F * _manager.max_mana) return;

        foreach (Enemy e in _manager.enemies_in_arena)
        {
            if (e == null) continue;
            e.Hype();
        }

        _manager.RemoveMana(0.5F * _manager.max_mana);
        _manager.ResetActionTimer();
        PlayComment();
    }

    public void DiscoUltimate()
    {
        if (!_manager.PlayerReady() || current_type != MusicTypes.Disco) return;
        if (_manager.current_mana < _manager.max_mana) return;

        foreach (Enemy e in _manager.enemies_in_arena)
        {
            if (e == null) continue;
            e.type = MusicTypes.Disco;
        }

        _manager.RemoveAllMana();
        _manager.ResetActionTimer();
        PlayComment();
    }



    // FUNK ABILITIES
    public void FunkBasic()
    {
        if (!_manager.PlayerReady() || current_type != MusicTypes.Funk) return;
        if (_manager.SelectedEnemy() != null) _manager.SelectedEnemy().Hype();


        _manager.ResetActionTimer();
        PlayComment();
    }

    public void FunkFirst()
    {
        if (!_manager.PlayerReady() || current_type != MusicTypes.Funk) return;
        if (_manager.current_mana < 0.25F * _manager.max_mana) return;
        if (_manager.SelectedEnemy() == null) return;

        _manager.SelectedEnemy().Hype();
        _manager.SelectedEnemy().Hype();

        _manager.RemoveMana(0.25F * _manager.max_mana);
        _manager.ResetActionTimer();
        PlayComment();
    }

    public void FunkSecond()
    {
        if (!_manager.PlayerReady() || current_type != MusicTypes.Funk) return;
        if (_manager.current_mana < 0.5F * _manager.max_mana) return;

        foreach (Enemy e in _manager.enemies_in_arena)
        {
            if (e == null) continue;
            if (e.boredom > 0 && e.type != MusicTypes.Funk) e.boredom -= 1;
        }

        _manager.RemoveMana(0.5F * _manager.max_mana);
        _manager.ResetActionTimer();
        PlayComment();
    }

    public void FunkUltimate()
    {
        if (!_manager.PlayerReady() || current_type != MusicTypes.Funk) return;
        if (_manager.current_mana < _manager.max_mana) return;
        if (_manager.SelectedEnemy() == null) return;
        if (_manager.SelectedEnemy().type == MusicTypes.Funk) return;

        _manager.SelectedEnemy().boredom = 0;

        _manager.RemoveAllMana();
        _manager.ResetActionTimer();
        PlayComment();
    }

    private void Q_Action()
    {
        switch(current_type)
        {
            case MusicTypes.Disco:
                DiscoBasic();
                break;

            case MusicTypes.Funk:
                FunkBasic();
                break;

            case MusicTypes.Rock:
                RockBasic();
                break;
        }
    }

    private void W_Action()
    {
        switch (current_type)
        {
            case MusicTypes.Disco:
                DiscoFirst();
                break;

            case MusicTypes.Funk:
                FunkFirst();
                break;

            case MusicTypes.Rock:
                RockFirst();
                break;
        }
    }

    private void E_Action()
    {
        switch (current_type)
        {
            case MusicTypes.Disco:
                DiscoSecond();
                break;

            case MusicTypes.Funk:
                FunkSecond();
                break;

            case MusicTypes.Rock:
                RockSecond();
                break;
        }
    }

    private void R_Action()
    {
        switch (current_type)
        {
            case MusicTypes.Disco:
                DiscoUltimate();
                break;

            case MusicTypes.Funk:
                FunkUltimate();
                break;

            case MusicTypes.Rock:
                RockUltimate();
                break;
        }

    }

    void PlayComment()
    {
        int rng = Random.Range(0, clips.Length);
        audio_source.clip = clips[rng];
        audio_source.Play();
        audio_source.pitch = Random.Range(0.9F, 1.1F);
    }

}
