using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EnenyHP : MonoBehaviour
{
    [SerializeField] private RectTransform fillTransform;

    private EnemyAI _enemyCharacter;
    private Camera _camera;
    [SerializeField] private GameObject Canvas;

    private void Awake()
    {
        _enemyCharacter = GetComponentInParent<EnemyAI>();
        _camera = Camera.main;
    }

    private void Update()
    {
        float percent = 0;
        percent = _enemyCharacter.HitPointPercent;
        if (percent != 1)
            Canvas.SetActive(true);
        else
            Canvas.SetActive(false);
        if (fillTransform != null)
            fillTransform.anchorMin = new Vector2(1f - _enemyCharacter.HitPointPercent, 0f);
        transform.LookAt(_camera.transform.position);
    }
}
