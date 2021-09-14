using UnityEngine;
using System.Collections;

public class healPlayer : MonoBehaviour
{
    void Die() //player death
	{
        animator.SetBool("dead", true);
        enemyAI.enabled = false;
        boxCollider2D.enabled = false;
		Instantiate(bloodSplashContainer, transform.position, Quaternion.identity);       
        
	}
}