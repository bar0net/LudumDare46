using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Manager : MonoBehaviour
{
    public static float _ACT_TRIGGER_ = 15F;

    [Header("Player")]
    public float max_mana = 99F;
    public float mana_generation = 8F;
    public float current_mana = 0;

    public float speed = 6F;
    
    [Header("UI")]
    public Gradient music_colors;
    public Transform lamp_light_transform;
    public Transform action_light_transform;
    public SpriteRenderer[] color_renderers;
    public UnityEngine.Experimental.Rendering.Universal.Light2D[] color_lights;
    public Transform enemy_indicator;
    public UnityEngine.UI.Text[] skill_help_texts;

    [Header("Enemy Tracking")]
    public Smoke[] placement;
    public Enemy[] enemies_in_arena = new Enemy[3];

    public GameObject[] supporters;
    int next_supporter_in = 0;

    public float arrival_min = 20F;
    public float arrival_max = 30F;

    public float leave_min = 25F;
    public float leave_max = 35F;

    [Header("EnemyList")]
    public EnemyPool.EnemyTypes[] enemy_list;
    int enemy_list_position = 0;

    //---
    int selected_enemy = -1;
    float action_timer = 0;
    EnemyPool _enemy_pool;
    float arrival_objective = 8F;
    float arrival_timer = 0;

    float leave_objective = 30F;
    public float leave_timer = 0;

    ArrivalIndicator _arrival;


    // Start is called before the first frame update
    void Start()
    {
        _enemy_pool = GameObject.FindObjectOfType<EnemyPool>();
        _arrival = GameObject.FindObjectOfType<ArrivalIndicator>();

        CreateEnemy();

        SupporterIn();
        SupporterIn();
        //CreateEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) Victory();
        if (Input.GetKeyDown(KeyCode.F2)) GameOver();

        if (current_mana < max_mana)
        {
            current_mana += mana_generation * Time.deltaTime;
            if (current_mana > max_mana) current_mana = max_mana;
        }
        lamp_light_transform.localScale = new Vector3(1F, current_mana / max_mana, 1F);

        if (action_timer < _ACT_TRIGGER_) action_timer += speed * Time.deltaTime;
        action_light_transform.localScale = new Vector3(1F, Mathf.Clamp(action_timer / _ACT_TRIGGER_, 0, 1));

        arrival_timer += Time.deltaTime;
        if (arrival_timer > arrival_objective)
        {
            CreateEnemy();
            arrival_timer = 0;
            arrival_objective = Random.Range(arrival_min, arrival_max);
        }
        _arrival.UpdatePosition(arrival_timer / arrival_objective);

        leave_timer += Time.deltaTime;
        if (leave_timer > leave_objective)
        {
            SupporterOut();
            leave_timer = 0;
            leave_objective = Random.Range(leave_min, leave_max);
        }
    }


    public bool HasEnoughMana(float amount)
    {
        return current_mana >= amount;
    }

    public void AddMana(float amount)
    {
        current_mana += amount;
        if (current_mana > max_mana) current_mana = max_mana;
    }

    public void RemoveMana(float amount)
    {
        if (current_mana > amount) current_mana -= amount;
    }

    public void RemoveAllMana()
    {
        current_mana = 0;
    }

    public void SetColors(float ratio)
    {
        Color current_color = music_colors.Evaluate(ratio);
        foreach (UnityEngine.Experimental.Rendering.Universal.Light2D light in color_lights) 
        {
            light.color = current_color;
        }

        foreach (SpriteRenderer sr in color_renderers)
        {
            sr.color = current_color;
        }
    }

    public void CreateEnemy()
    {
        if (enemy_list_position >= enemy_list.Length) return;
        enemy_list_position++;

        int i = Random.Range(0, 3);
        for (int j = 0; j < 3; j++)
        {
            if (enemies_in_arena[ (i+j)%3 ] == null)
            {
                //Birth( (i+j)%3, enemy_list[Random.Range(0,enemy_list.Length)]);
                
                Birth((i + j) % 3, _enemy_pool.Get(enemy_list[enemy_list_position-1]));

                if (enemy_list_position >= enemy_list.Length)
                {
                    //TODO: WHAT TO DO WHEN THE LIST IS OVER?
                    Victory();
                }

                return;
            }
        }
    }

    public void Birth(int index, GameObject character)
    {
        //GameObject go = (GameObject)Instantiate(character, placement[index].transform.position, Quaternion.identity);

        GameObject go = character;
        go.transform.position = placement[index].transform.position;
        placement[index].character = go;
        placement[index].creation = true;
        placement[index].gameObject.SetActive(true);
        placement[index].Load();
        enemies_in_arena[index] = go.GetComponent<Enemy>();
        go.SetActive(false);

        if (selected_enemy == -1) EnemySelect(index);
    }

    public void EnemySelect(int index)
    {
        if (index == -1) enemy_indicator.position = Vector3.left * 30F;
        else enemy_indicator.position = placement[index].transform.position + Vector3.up;

        selected_enemy = index;
    }

    public void SelectNext()
    {
        if (enemies_in_arena[(selected_enemy + 1) % 3] != null) EnemySelect((selected_enemy + 1) % 3);
        else if (enemies_in_arena[(selected_enemy + 2) % 3] != null) EnemySelect((selected_enemy + 2) % 3);
    }

    public void SelectPrevious()
    {
        if (enemies_in_arena[(selected_enemy + 2) % 3] != null) EnemySelect((selected_enemy + 2) % 3);
        else if (enemies_in_arena[(selected_enemy + 1) % 3] != null) EnemySelect((selected_enemy + 1) % 3);
    }

    public bool PlayerReady() { return action_timer >= _ACT_TRIGGER_; }
    public void ResetActionTimer() { action_timer = 0; }

    public Enemy SelectedEnemy()
    {
        if (selected_enemy == -1) return null;
        else return enemies_in_arena[selected_enemy];
    }

    public void CheckSelector()
    {
        if (enemies_in_arena[selected_enemy] != null) return;
        SelectNext();

        if (enemies_in_arena[selected_enemy] == null) EnemySelect(-1);
    }

    public void SupporterIn()
    {
        leave_timer -= 3;

        if (next_supporter_in < supporters.Length)
        {
            supporters[next_supporter_in].GetComponentInChildren<Animator>().SetTrigger("In");
            next_supporter_in++;
        }
    }

    public void SupporterOut()
    {
        if (next_supporter_in > 0)
        {
            supporters[next_supporter_in-1].GetComponentInChildren<Animator>().SetTrigger("Out");
            next_supporter_in--;
        }

        if (next_supporter_in == 0) GameOver();
    }

    public void GameOver()
    {
        FindObjectOfType<LevelManager>().LoadLevel("GameOver");
    }

    public void Victory()
    {
        FindObjectOfType<LevelManager>().LoadLevel("Victory");
    }

    public void SetHelpText(int index, string desc)
    {
        skill_help_texts[index].text = desc;
    }
}

