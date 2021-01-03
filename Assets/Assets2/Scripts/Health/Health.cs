using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;

    private float currentHealth;

    void Start()
    {
        currentHealth = startingHealth;
    }

    public void Damage(float damageVal)
    {
        currentHealth -= damageVal;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        print("ded");
    }
}
