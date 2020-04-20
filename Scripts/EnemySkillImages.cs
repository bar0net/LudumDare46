using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillImages : MonoBehaviour
{
    [System.Serializable]
    public struct Pair
    {
        public EnemySkillList skill;
        public Sprite sprite;
        public string description;
    }

    public Pair[] skill_list;

    Dictionary<EnemySkillList, Sprite> sprites = new Dictionary<EnemySkillList, Sprite>();
    Dictionary<EnemySkillList, string> descriptions = new Dictionary<EnemySkillList, string>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Pair item in skill_list)
        {
            sprites[item.skill] = item.sprite;
            descriptions[item.skill] = item.description;
        }
    }

    public Sprite Get(EnemySkillList skill)
    {
        if (sprites.ContainsKey(skill))
        {
            return sprites[skill];
        }
        else return skill_list[0].sprite;
    }

    public string Desc(EnemySkillList skill)
    {
        if (sprites.ContainsKey(skill))
        {
            return descriptions[skill];
        }
        else return descriptions[EnemySkillList.None];
    }
}
