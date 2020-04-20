using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tutorials : MonoBehaviour
{

    public GameObject[] screens;
    int current = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) && current < screens.Length - 1)
        {
            screens[current].SetActive(false);
            current++;
            screens[current].SetActive(true);
        }
    }
}
