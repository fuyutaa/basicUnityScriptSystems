using UnityEngine;
using System.Collections;
/*
CONTENT :
	- shoots on function call. 
	- Can be used with buttons, key presses.

1. Store in a list all enemies with Enemy tag 
2. Take the nearest enemy in the list
3. Checks if nearest enemy is in range
4. Calls corresponding function for attacking (Shoot / Laser)
*/
public class executionnerAttack : MonoBehaviour {

	private Transform target;
	private Enemy targetEnemy;
	GameObject nearestEnemy;

	[Header("General")]
	public float range = 15f;
	public string keyForAttack;

	[Header("Use Bullets")]
	public GameObject bulletPrefab;
	public float fireRate;
	public float fireCountdown;

	[Header("Use Laser")]
	public bool useLaser;

	public int damageOverTime = 30;

	public LineRenderer lineRenderer;
	public Light impactLight;
	public GameObject impactEffect;

	[Header("Unity Setup Fields")]

	public string enemyTag = "Enemy";

	[Header("Optional : specific firePoint (see script)")]
	public Transform firePoint; // optional, gives the possibility to have a specific firePoint. If no firePoint is defined, it will by default fire from the script's holder transform

	bool buttonPressed;
	bool usingFireRate = true; // the script will do fireRate operations even if fireRate = 0, making the shooting system glitchy. This avoid making these operations and making it working if there's no fireRate.

	public void Start()
	{
		if(fireRate == 0)
		{
			usingFireRate = false;
		}		
	}

	public void ButtonPressAttack() 
	{
		fireCountdown = fireRate;
		UpdateTarget();
		buttonPressed = true;
	}
	
	void UpdateTarget ()
	{
		firePoint = this.transform;
		float shortestDistance = Mathf.Infinity;

		GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
		nearestEnemy = null;

		//Finding and storing nearest enemy (and its distance)
		foreach (GameObject enemy in enemies)
		{
			float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
			if (distanceToEnemy < shortestDistance) 
			{
				shortestDistance = distanceToEnemy;
				nearestEnemy = enemy;
			}
		}

		if (nearestEnemy != null && shortestDistance <= range)
		{
			target = nearestEnemy.transform;
			targetEnemy = nearestEnemy.GetComponent<Enemy>();
		} 
		else
		{
			target = null;
		}

	}

	void Update() 
	{
		if (Input.GetButtonDown(keyForAttack))
		{
			ButtonPressAttack();
		}

		if (target == null)
		{
			if (useLaser)
			{
				if (lineRenderer.enabled)
				{
					lineRenderer.enabled = false;
					Destroy(impactEffect);
					impactLight.enabled = false;
				}
			}

			return;
		}

		if (useLaser && buttonPressed)
		{
			Laser();
		} 
		else // if using bullets
		{
			if (fireCountdown <= 0f && buttonPressed)
			{
				fireCountdown = fireRate;
				Shoot();
				target = null;
			}

			if(usingFireRate)
			{
				fireCountdown -= Time.deltaTime;
			}
		}
	}

	void Laser ()
	{
		targetEnemy.TakeDamage(damageOverTime);
		//targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
		//targetEnemy.Slow(slowAmount);

		if (!lineRenderer.enabled)
		{
			lineRenderer.enabled = true;
			GameObject impactEffect = (GameObject)Instantiate(bulletPrefab, targetEnemy.transform.position, this.transform.rotation);
			impactLight.enabled = true;
		}

		lineRenderer.SetPosition(0, firePoint.position);
		lineRenderer.SetPosition(1, target.position);

		Vector3 dir = firePoint.position - target.position;

		impactEffect.transform.position = target.position + dir.normalized;

		impactEffect.transform.rotation = Quaternion.LookRotation(dir);
	}

	void Shoot()
	{
		GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
		Bullet bullet = bulletGO.GetComponent<Bullet>();

		if (bullet != null)
			bullet.Init(target, nearestEnemy);
	}

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, range);
	}
}