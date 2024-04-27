using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SingleTimeEffect : MonoBehaviour
{

    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator.Play("Entry", -1, normalizedTime: 0f);
    }

    public void FinishEffect() 
    {
        this.Despawn();
    }
}
