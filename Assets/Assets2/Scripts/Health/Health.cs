using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Debuffs 
{
    none,
    slow, 
}

public class Health : MonoBehaviour
{
    public delegate void OnDamaged(Debuffs debuff);
    public event OnDamaged TookDamage;

    [SerializeField] private float startingHealth;
    [SerializeField] public bool invulnerable;

    public float currentHealth;

    void Start()
    {
        currentHealth = startingHealth;
    }

    public void Damage(float damageVal, Debuffs debuff = Debuffs.none)
    {
        if (!invulnerable)
        {
            TookDamage?.Invoke(debuff);

            currentHealth -= damageVal;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        print("ded");
    }
}
