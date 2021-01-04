using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Slider healthbar;

    [SerializeField] private GameObject hurtParticle;

    [HideInInspector] public bool isDead;
    //private float currentHealth;

    void Start()
    {
        isDead = false;
        currentHealth = startingHealth;

        if (healthbar != null)
        {
            healthbar.maxValue = startingHealth;
            healthbar.value = currentHealth;
        }
    }

    public void Damage(float damageVal, Debuffs debuff = Debuffs.none)
    {
        if (!invulnerable)
        {
            TookDamage?.Invoke(debuff);

            Instantiate(hurtParticle, transform.position, transform.rotation);

            currentHealth -= damageVal;

            if (healthbar != null)
                healthbar.value = currentHealth;

            if (currentHealth <= 0)
                Die();
        }
    }

    private void Die()
    {
        if (gameObject.CompareTag("Player"))
        {
            print("ded");
            isDead = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
