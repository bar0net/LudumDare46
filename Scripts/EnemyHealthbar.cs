using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    public static float _MAX_HYPEBAR_WIDTH_ = 107F;
    public static float _MAX_BOREBAR_WIDTH_ = 104F;
    public static float _MAX_ACTBAR_WIDTH_  = 138F;

    public RectTransform hype_mask;
    public RectTransform bore_mask;
    public RectTransform act_mask;

    public void UpdateBars(float hype_ratio, float bore_ratio, float act_ratio)
    {
        hype_mask.sizeDelta = new Vector2(hype_ratio * _MAX_HYPEBAR_WIDTH_, hype_mask.rect.height);
        bore_mask.sizeDelta = new Vector2(bore_ratio * _MAX_BOREBAR_WIDTH_, bore_mask.rect.height);
        act_mask.sizeDelta  = new Vector2(act_ratio * _MAX_ACTBAR_WIDTH_, act_mask.rect.height);
    }
}
