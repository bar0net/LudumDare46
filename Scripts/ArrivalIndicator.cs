using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrivalIndicator : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public Transform character;

    public void UpdatePosition(float ratio)
    {
        character.transform.position = Vector3.Lerp(start.position, end.position, ratio);
    }
}
