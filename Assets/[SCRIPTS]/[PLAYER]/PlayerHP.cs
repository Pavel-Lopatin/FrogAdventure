using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public static int currHP;
    [SerializeField] private int maxHP;

    [SerializeField] private Image[] livesImages;

    [SerializeField] private Sprite fullLive;
    [SerializeField] private Sprite emptyLive;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        currHP = maxHP;

        // настройка картинок
        for (int i = 0; i < livesImages.Length; ++i)
        {
            if (i < currHP) livesImages[i].sprite = fullLive;
            else livesImages[i].sprite = emptyLive;

            if (i < maxHP) livesImages[i].enabled = true;
            else livesImages[i].enabled = false;
        }
    }

    public void TakeDamage(int amount)
    {
        currHP = Mathf.Clamp(currHP + amount, 0, maxHP);

        for (int i = 0; i < livesImages.Length; ++i)
        {
            if (i < currHP) livesImages[i].sprite = fullLive;
            else livesImages[i].sprite = emptyLive;
        }

        if (currHP == 0)
        {
            Destroy(gameObject);
        }
    }
}
