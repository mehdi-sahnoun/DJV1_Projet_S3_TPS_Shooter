using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private RectTransform fillTransform;
    [SerializeField] private GameObject _player;

    // Update is called once per frame
    void Update()
    {
        float percent = 0;
        percent = _player.GetComponent<PlayerCaracter>().HitPointPercent;
        if (fillTransform != null)
            fillTransform.anchorMax = new Vector2(percent, 1f);
        if(percent > 1)
            fillTransform.anchorMax = new Vector2(1f, 1f);
    }
}
