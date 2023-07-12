using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBottomCheck : MonoBehaviour
{
    private PlayerController controller;
    private int damage;

    private void Start()
    {
        controller = GetComponentInParent<PlayerController>();
        damage = controller.damage;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyController enemy = collision.gameObject.GetComponentInParent<EnemyController>();

        if (enemy)
        {
            StartCoroutine(enemy.TakeHit(damage));
            StartCoroutine(controller.Jump());
        }
    }
}
