using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public float invincibilityTimeAfterHit = 1f;
    public float invincibilityFlashDelay = 0.2f;
    public bool isInvincible = false;

    public SpriteRenderer graphics;
    public HealthBar healthBar;

    public AudioClip hitSound;

    public static PlayerHealth instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("There's more than a single instance in the scene.");
            return;
        }

        instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        player.instance.animator.SetInteger("currentHealth", currentHealth);
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

        HealthDataSender();
    }

    public void SetHealth(int amount)
    {
        currentHealth = amount;
        HealthDataSender();
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            // AudioManager.instance.PlayClipAt(hitSound, transform.position);
            currentHealth -= damage;
            HealthDataSender();

            if(currentHealth <= 0)
            {
                Die();
                return;
            }

            isInvincible = true;
            StartCoroutine(InvincibilityFlash());
            StartCoroutine(HandleInvincibilityDelay());
        }
    }

    public void Die()
    {
        player.instance.animator.SetTrigger("Die");
        player.instance.rb.bodyType = RigidbodyType2D.Kinematic;
        player.instance.rb.velocity = Vector3.zero;
        player.instance.playerCollider.enabled = false;
        // GameOverManager.instance.OnPlayerDeath();
    }

    public void Respawn()
    {
        player.instance.enabled = true;
        player.instance.animator.SetTrigger("Respawn");
        player.instance.rb.bodyType = RigidbodyType2D.Dynamic;
        player.instance.playerCollider.enabled = true;
        currentHealth = maxHealth;
        HealthDataSender();
    }

    public IEnumerator InvincibilityFlash()
    {
        while (isInvincible)
        {
            graphics.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(invincibilityFlashDelay);
            graphics.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(invincibilityFlashDelay);
        }
    }

    public IEnumerator HandleInvincibilityDelay()
    {
        yield return new WaitForSeconds(invincibilityTimeAfterHit);
        isInvincible = false;
    }

    public void HealthDataSender()
    {
        healthBar.SetHealth(currentHealth);
        player.instance.animator.SetInteger("currentHealth", currentHealth);

    }
}