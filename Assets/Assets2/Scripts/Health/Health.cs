﻿using System;
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
    [SerializeField] private float currentHealth;
    [SerializeField] public bool invulnerable;

    [HideInInspector] public bool isDead;
    private float currentHealth;

    void Start()
    {
        isDead = false;
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
        isDead = true;
    }
}
