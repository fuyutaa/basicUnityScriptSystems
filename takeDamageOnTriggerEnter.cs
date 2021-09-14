using UnityEngine;
using System.Collections;

public class healPlayer : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other) 
        {
            if(other.gameObject.CompareTag("player"))
            {
                TakeDamage(0);
            }
        }
}