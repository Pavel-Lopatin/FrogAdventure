using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSideCheck : MonoBehaviour
{
    private PlayerController controller;
    [SerializeField] private bool isFront;

    private void Start()
    {
        controller = GetComponentInParent<PlayerController>();

        if (transform.localPosition.x > 0) isFront = true;
        else isFront = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyController enemy = collision.gameObject.GetComponentInParent<EnemyController>();

        if (enemy)
        {
            if (isFront) controller.TakeDamage(enemy.damage, enemy.attackForce, 1);
            else controller.TakeDamage(enemy.damage, enemy.attackForce, 2);
        }
    }
}
