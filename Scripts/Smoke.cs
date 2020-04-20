using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    public GameObject character;
    public bool trigger = false;
    public bool end = false;
    public int position = 0;

    public bool creation = false;

    Animator _anim;
    Manager _m;
    EnemyPool _epool;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _m = FindObjectOfType<Manager>();
        _epool = FindObjectOfType<EnemyPool>();
    }

    public void Load()
    {
        _anim.SetTrigger("Start");
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger && character != null)
        {
            character.SetActive(creation);

            if (creation)
            {
                Enemy e = character.GetComponent<Enemy>();
                if (e != null) e.placement = position;
            }
            else _epool.Store(character);

            character = null;
        }

        if (end)
        {
            if (!creation)
            {
                _m.enemies_in_arena[position] = null;
                _m.CheckSelector();
            }

            this.gameObject.SetActive(false);
        }
    }
}
