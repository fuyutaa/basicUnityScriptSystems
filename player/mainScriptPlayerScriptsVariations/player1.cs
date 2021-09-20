using UnityEngine;
using System.Collections;

/*
CONTAINS :
    - HealthSystem (TakeDamage, SetHealth, HealPlayer, HealthDataSender, Die)
    - Flip X system to look in direction where the player is going, (works with velocity)
    - Dialogue System links (works with the DialogueSystem of SemagGames, re-wrote and up on my github.)
    - Sends Speed to Animator
*/
public class player : MonoBehaviour
{
    public static player instance;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D playerCollider;
    MovePlayer movePlayer;

    //Movement stuff
    Vector2 movement;
    public GameObject joystickArea;

    // Dialogue stuff
    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;
    public IInteractible Interactible { get; set;}

    // Health system
	public int currentHealth = 100;
    public int maxHealth = 100;
    public bool died;
    public bool isInvincible = false;

    public HealthBar healthBar;
    public AudioClip hitSound;
    public GameObject bloodSplashContainer; // game object containing blood splash effect because we cannot SetActive() a particle system

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("there's more than a single instance of player in the scene");
            return;
        }

        instance = this;
    }

    public void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        playerCollider = this.GetComponent<CapsuleCollider2D>();
        movePlayer = this.GetComponent<MovePlayer>();

        //Health System
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        player.instance.animator.SetInteger("currentHealth", currentHealth);
    }

    void Update()
    {
        if(dialogueUI.IsOpen) return;

        if(Input.GetKeyDown(KeyCode.E))
        {
            Interactible?.Interact(this);
        }

        Flip(rb.velocity.x);
    }

    void FixedUpdate()
    {
        animator.SetFloat("Speed", rb.velocity.magnitude);
    }

    void Flip(float _velocity)
    {
        if (_velocity > 0.1f)
        {
            spriteRenderer.flipX = false;
        }else if(_velocity < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }

    // Health system
    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            // AudioManager.instance.PlayClipAt(hitSound, transform.position);
            GameObject blood = Instantiate(bloodSplashContainer, transform.position, Quaternion.identity);
            currentHealth -= damage;
            HealthDataSender();

            if(currentHealth <= 0)
            {
                Die();
                return;
            }

            isInvincible = true;
            Destroy(blood);
        }
    }

    public void SetHealth(int amount)
    {
        currentHealth = amount;
        HealthDataSender();
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

    public void Die()
    {
        animator.Play("executionnerDie");
        movePlayer.enabled = false;
        player.instance.rb.bodyType = RigidbodyType2D.Kinematic;
        player.instance.rb.velocity = Vector3.zero;
        player.instance.playerCollider.enabled = false;

        died = true;
    }

    public void HealthDataSender()
    {
        healthBar.SetHealth(currentHealth);
        player.instance.animator.SetInteger("currentHealth", currentHealth);

    }
}
