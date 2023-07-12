using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedKit : MonoBehaviour
{
    [Range(1, 3)]
    [SerializeField] private int recoveryHP;

    // переменная, чтобы не писать миллион доп условий
    private bool canHeal;
    private Animator animator;

    private void Start()
    {
        canHeal = true;
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHP playerHP = collision.GetComponent<PlayerHP>();

            if (playerHP && canHeal)
            {
                canHeal = false;
                playerHP.TakeDamage(recoveryHP);

                animator.SetTrigger("Collected");

                Destroy(gameObject, 1f);
            }
        }
    }
}
