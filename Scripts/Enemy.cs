using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [System.Serializable]
    public struct LearntSkills
    {
        public EnemySkillList skill;
        public int weight; // The probability of using the skill (higher, more probable)
    }

    [Header("Atributes")]
    public int enemy_type = -1;
    public ActionMenu.MusicTypes type;
    public float bonus_mana = 5F;

    public int hype = 0;
    public int max_hype = 5;

    public int boredom = 0;
    public int max_boredom = 5;

    public float speed = 3F;
    public int placement = 0;

    public LearntSkills[] available_skills;

    [Header("Status")]
    public int entranced = 0;
    public int stunned = 0;

    public int extra_bore_timer = 0;
    public int extra_hype_timer = 0;
    public int hype_resist_timer = 0;

    public bool acting = false;
    public bool acting_end = false;

    [Header("UI")]
    public UnityEngine.UI.Image skill_image;

    ActionMenu _am;
    Manager _m;
    Animator _anim;
    EnemyHealthbar _healthbar;
    EnemySkillImages _skill_img;
    AudioSource _as;

    int sum_of_weights = 0;
    public EnemySkill next_skill = null;

    float action_timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        _am = GameObject.FindObjectOfType<ActionMenu>();
        _m  = GameObject.FindObjectOfType<Manager>();
        _anim = GetComponent<Animator>();
        _healthbar = GetComponentInChildren<EnemyHealthbar>();
        _skill_img = FindObjectOfType<EnemySkillImages>();
        _as = GetComponent<AudioSource>();

        if (_anim != null)
        {
            _anim.SetBool("Hyped", false);
            _anim.SetBool("Bored", false);
        }

        foreach (LearntSkills skill in available_skills) sum_of_weights += skill.weight;
        if (_healthbar != null) _healthbar.UpdateBars(hype / max_hype, boredom / max_boredom, action_timer / Manager._ACT_TRIGGER_);

        GetNextSkill();

    }

    private void OnEnable()
    {
        //Reset On Enable
        boredom = 0;
        hype = 0;
        acting = false;
        acting_end = false;

        entranced = 0;
        stunned = 0;

        extra_bore_timer = 0;
        extra_hype_timer = 0;
        hype_resist_timer = 0;

        action_timer = 0;
}

    // Update is called once per frame
    void Update()
    {
        if ( (_am.CurrentType() == type || stunned > 0) && skill_image.enabled) skill_image.enabled = false;
        else if ((_am.CurrentType() != type && stunned == 0) && !skill_image.enabled) skill_image.enabled = true;


        if (_am.CurrentType() == type)
        {
            _m.AddMana(bonus_mana * Time.deltaTime);
        }
        _anim.SetBool("Vibe", _am.CurrentType() == type);

        if (_anim != null)
        {
            if (hype    > max_hype / 2)    _anim.SetBool("Hyped", true);
            if (boredom > max_boredom / 2) _anim.SetBool("Bored", true);
        }

        action_timer += speed * Time.deltaTime;
        if (action_timer > Manager._ACT_TRIGGER_ && !acting) Act();
        if (acting_end) EndTurn();
    }

    private void LateUpdate()
    {
        if (_healthbar != null) _healthbar.UpdateBars((float)hype / max_hype, (float)boredom / max_boredom, action_timer / Manager._ACT_TRIGGER_);
    }

    public void Act()
    {
        if (entranced > 0 || stunned > 0 || next_skill == null || _am.CurrentType() == type)
        {
            EndTurn();
            return;
        }

        _anim.SetTrigger("Act");
        _as.Play();
        next_skill.Use(_m, this);
        next_skill = null;
        acting = true;
    }

    void GetNextSkill()
    {
        if (available_skills.Length == 0 || sum_of_weights == 0) return;

        int rng = Random.Range(0, sum_of_weights);
        for (int i = 0; i < available_skills.Length; i++)
        {
            rng -= available_skills[i].weight;
            if (rng < 0)
            {
                next_skill = EnemySkill.GetSkill(available_skills[i].skill);
                break;
            }
        }
        if (next_skill == null) next_skill = EnemySkill.GetSkill(available_skills[available_skills.Length - 1].skill);

        if (!next_skill.CanUse(this)) next_skill = new Tantrum();

        skill_image.sprite = _skill_img.Get(next_skill.type);
        _m.SetHelpText(placement, _skill_img.Desc(next_skill.type));
    }

    public void EndTurn()
    {
        acting = false;
        acting_end = false;
        action_timer = 0;

        if (entranced > 0) entranced -= 1;
        if (stunned > 0) stunned -= 1;

        if (next_skill == null) GetNextSkill();
    }

    public void Bore()
    {
        if (_am == null) _am = GameObject.FindObjectOfType<ActionMenu>();
        if (entranced > 0 || _am.CurrentType() == type) return;

        if (extra_bore_timer > 0)
        {
            boredom += 2;
            extra_bore_timer -= 1;
        }
        else boredom += 1;

        if (boredom >= max_boredom) Defeat();
    }

    public void Hype()
    {
        if (_am == null) _am = GameObject.FindObjectOfType<ActionMenu>();
        if (hype_resist_timer > 0 || entranced > 0 || _am.CurrentType() == type) return;

        if (extra_hype_timer > 0)
        {
            hype += 2;
            extra_hype_timer -= 1;
        }
        else hype += 1;

        if (hype >= max_hype) Victory();
    }

    public void Defeat()
    {
        Smoke s = _m.placement[placement];
        s.creation = false;
        s.character = this.gameObject;
        s.gameObject.SetActive(true);
        s.Load();

        //TODO: WHAT HAPPENS ON DEFEAT
        _m.leave_timer += 2F;
    }

    public void Victory()
    {
        Smoke s = _m.placement[placement];
        s.creation = false;
        s.character = this.gameObject;
        s.gameObject.SetActive(true);
        s.Load();

        //TODO: WHAT HAPPENS ON VICTORY
        _m.SupporterIn();

    }

    public Enemy CheckLeft(bool go_around = false)
    {
        if (placement == 0 && !go_around) return null;
        if (_m.enemies_in_arena[(placement + 2) % 3] == null) return null;

        return _m.enemies_in_arena[(placement + 2)%3];
    }
    public Enemy CheckRight(bool go_around = false)
    {
        if (placement == 2 && !go_around) return null;
        if (_m.enemies_in_arena[(placement + 1) % 3] == null) return null;

        return _m.enemies_in_arena[(placement + 1)%3];
    }

}
