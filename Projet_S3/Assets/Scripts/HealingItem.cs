using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : MonoBehaviour
{
    private Animator animator;
    private int collectTriggerHash;
    private bool consumed = false;

    void Awake()
    {
        // R�cup�re l'Animator attach� � l'objet et le hash du trigger "Collect"
        animator = GetComponent<Animator>();
        collectTriggerHash = Animator.StringToHash("Collect");
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("hi");
        if (other.gameObject.TryGetComponent<PlayerCaracter>(out var player) && !consumed)
        {
            consumed = true;

            animator.SetTrigger(collectTriggerHash);
            /////maybe deactivate the box collider

            /////heal logic
            player.ApplyDamage(-5);

            Destroy(this.gameObject, 2.0f);
        }
    }
}
