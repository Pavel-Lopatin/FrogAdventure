using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    private int currHP;
    [SerializeField] private int maxHP;

    private EnemyController controller;

    private void Start()
    {
        currHP = maxHP;
        controller = GetComponent<EnemyController>();
    }

    public void TakeDamage(int amount)
    {
        currHP = Mathf.Clamp(currHP - amount, 0, maxHP);

        if (currHP >= 1)
        {
            controller._animator.SetTrigger("Hit");
        }
        else if (currHP == 0)
        {
            // я не понимаю  почему эта херня отключает Spritr Renderer!!!!!
            /*
            for (int i = 0; i < controller.childObjects.Length; ++i)
            {
                controller.childObjects[i].gameObject.SetActive(false);
            }
            */

            controller.collider2D.enabled = false;
            controller._isTakingHit = true;
            controller._animator.SetTrigger("Die");

            Destroy(gameObject, 5f);
        }
    }
}
