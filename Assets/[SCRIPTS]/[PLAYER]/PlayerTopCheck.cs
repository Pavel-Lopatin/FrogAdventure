using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTopCheck : MonoBehaviour
{
    private PlayerController controller;

    private void Start()
    {
        controller = GetComponentInParent<PlayerController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyController enemy = collision.gameObject.GetComponentInParent<EnemyController>();

        if (enemy)
        {
            controller.TakeDamage(enemy.damage, enemy.attackForce, 3);
        }
    }
}

