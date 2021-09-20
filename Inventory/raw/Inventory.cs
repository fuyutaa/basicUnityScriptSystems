using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.VFX;
public class Inventory : MonoBehaviour
{

    public List<Item> content = new List<Item>();
    private int contentCurrentIndex = 0;
    private int maxItemAmount = 2;
    public Image itemImageUI;
    public TMP_Text itemNameUI;   
    public Sprite emptyItemImage;

    public GameObject C4;
    public GameObject Explosion;

    // player stuff
    public PlayerEffects playerEffects; 
    public GameObject player;
    public Animator playerAnimator;
    public player playerHealth;


    public static Inventory instance;
    public C4Manager c4Manager;
    public ParticleSystem explosion;
    public bool explosionOccurring;




    // delay times before disabling SpriteRenderer / Destroy(GameObject).
    public float explosionStopDelay = 5f;
    public float C4ExplosionDelay; 
    public float C4DespawnDelay = 0.3f;
    public float wallDestroyDelay;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de Inventory dans la scène");
            return;
        }

        instance = this;
    }

    private void Start()
    {
        UpdateInventoryUI();
        explosion = Explosion.GetComponent<ParticleSystem>();
        playerAnimator = player.GetComponent<Animator>();
        playerHealth = player.GetComponent<player>();
    }

    public void ConsumeItem()
    {
        if(content.Count == 0)
        {
            return;
        }
        Item currentItem = content[contentCurrentIndex];
        playerHealth.HealPlayer(currentItem.hpGiven);
        playerEffects.AddSpeed(currentItem.speedGiven, currentItem.speedDuration);


        if(currentItem.isExplosive == true)
        {
            Debug.Log("Item identified as explosive");
            playerCollisionManager playerCollisionManager = player.GetComponent<playerCollisionManager>();
            if (playerCollisionManager.destroyableWall == true)
            {
                explosionOccurring = true;
                StartCoroutine(ExplosionStart());
                Debug.Log("explosion started");
            }
            else 
            {
                Debug.Log("Cannot explode nearby wall");
                return;
            }
        }

        if(currentItem.isKatana) 
        { 
            playerAnimator.SetBool("katanaEquipped", true); 
        }
        
        content.Remove(currentItem);
        GetNextItem();
        UpdateInventoryUI();
    }

    public void GetNextItem()
    {
        if (content.Count == 0)
        {
            return;
        }

        contentCurrentIndex++;
        if(contentCurrentIndex > content.Count - 1)
        {
            contentCurrentIndex = 0;
        }
        UpdateInventoryUI();
    }

    public void dropItem()
    {
        Item currentItem = content[contentCurrentIndex];

        if (!currentItem.isKatana)
        {
            content.Remove(currentItem);
        }
    }

    public IEnumerator ExplosionStart() // blinking until it gets stopped by != explosionOccurring
    {
            // Debug.Log("spawning C4"); 
            GameObject CloneC4 = Instantiate(C4, transform.position = player.transform.position, Quaternion.identity);
            //CloneC4.transform.position = player.transform.position; // spawn C4 and explode timer
            CloneC4.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(C4ExplosionDelay);
            // Debug.Log("c4 timer finished, exploding.");
            Explosion.transform.position = CloneC4.transform.position;
            explosion.Play();

            yield return new WaitForSeconds(C4DespawnDelay); 
            CloneC4.GetComponent<SpriteRenderer>().enabled = false;

            c4Manager = CloneC4.GetComponent<C4Manager>();
            Destroy(c4Manager.wallToDestroy);
            
    }

    public void UpdateInventoryUI()
    {
        if(content.Count > 0)
        {
            itemImageUI.sprite = content[contentCurrentIndex].image;
            itemNameUI.text = content[contentCurrentIndex].name;
        }
        else
        {
            itemImageUI.sprite = emptyItemImage;
            itemNameUI.text = "";
        }
        
    }
}