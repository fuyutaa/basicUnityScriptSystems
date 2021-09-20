using UnityEngine;
using System.Collections;

public class healPlayer : MonoBehaviour
{
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
}