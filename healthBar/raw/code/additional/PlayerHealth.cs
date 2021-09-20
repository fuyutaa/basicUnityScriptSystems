using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;

    bool isInvincible = false;

    SpriteRenderer graphics;
    HealthBar healthBar;

    public AudioClip hitSound;

    public static PlayerHealth instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de PlayerHealth dans la scène");
            return;
        }

        instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(60);
        }
    }

    public void HealPlayer(int amount)
    {
        if((currentHealth + amount) > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;
        }

        healthBar.SetHealth(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            //AudioManager.instance.PlayClipAt(hitSound, transform.position);
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);

            if(currentHealth <= 0)
            {
                Die();
                return;
            }

            isInvincible = true;
        }
    }

    public void Die()
    {
        player.instance.enabled = false;
        player.instance.animator.SetTrigger("Die");
        player.instance.rb.bodyType = RigidbodyType2D.Kinematic;
        player.instance.rb.velocity = Vector3.zero;
        player.instance.playerCollider.enabled = false;
        //GameOverManager.instance.OnPlayerDeath();
    }
}