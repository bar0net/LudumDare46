using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource[] sources;
    public AudioSource[] fx;
    public float transition_time = 1f;

    public float max_volume = 0.5f;

    int current = 1;
    int previous = 1;

    float start_time = 0;

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i != current) sources[i].Pause();
            else sources[i].volume = max_volume;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (current == previous) return;

        float ratio = (Time.time - start_time) / transition_time;
        sources[previous].volume = max_volume * Mathf.Lerp(1f, 0, ratio);
        sources[current].volume  = max_volume * Mathf.Lerp(0, 1f, ratio);

        if (ratio >= 1)
        {
            sources[previous].Pause();
            previous = current;
        }
    }

    public void Fade(int type)
    {
        sources[previous].UnPause();
        start_time = Time.time;
        current = type;
        fx[type].Play();
    }
}
