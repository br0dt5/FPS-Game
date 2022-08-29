using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Visuals")]
    public Camera playerCamera;

    [Header("Gameplay")]
    public int initialHealth = 100;
    public int initialAmmo = 12;
    public float knockbackForce = 10;
    public float hurtDuration = 0.5f;

    private int health;
    public int Health { get { return health; } }

    private int ammo;
    public int Ammo { get { return ammo; } }

    public bool killed;
    public bool Killed { get { return killed; } }

    private bool isHurt;

    // Start is called before the first frame update
    void Start()
    {
        health = initialHealth;
        ammo = initialAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(ammo > 0 && !Killed)
            {
                ammo--;

                GameObject bulletObject = ObjectPoolingManager.Instance.GetBullet(true);
                bulletObject.transform.position = playerCamera.transform.position + playerCamera.transform.forward;
                bulletObject.transform.forward = playerCamera.transform.forward;
            }
        }
    }

    // Check for collisions.
    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<AmmoCrate>() != null)
        {
            // Collect ammo crates.
            AmmoCrate ammoCrate = other.GetComponent<AmmoCrate>();
            ammo += ammoCrate.ammo;

            Destroy(ammoCrate.gameObject);
        }
        else if(other.GetComponent<HealthCrate>() != null)
        {
            // Collect health crates.
            HealthCrate healthCrate = other.GetComponent<HealthCrate>();
            health += healthCrate.health;

            Destroy(healthCrate.gameObject);
        }
        if(!isHurt)
        {
            GameObject hazard = null;

            if(other.GetComponent<Enemy>() != null)
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (!enemy.Killed) 
                {
                    hazard = enemy.gameObject;
                    health -= enemy.damage;
                }
            }
            else if (other.GetComponent<Bullet>() != null)
            {
                Bullet bullet = other.GetComponent<Bullet>();
                if (bullet.ShotByPlayer == false)
                {
                    hazard = bullet.gameObject;
                    health -= bullet.damage;
                    bullet.gameObject.SetActive(false);
                }
            }

            if(hazard != null)
            {
                isHurt = true;

                // Perform the knockback effect.
                Vector3 hurtDirection = (transform.position - hazard.transform.position).normalized;
                Vector3 knockbackDirection = (hurtDirection + Vector3.up).normalized;
                GetComponent<ForceReceiver>().AddForce(knockbackDirection, knockbackForce);

                StartCoroutine(HurtRoutine());
            }

            if (health <= 0)
            {
                if (!killed) 
                {
                    killed = true;

                    OnKill();
                }
            }
        }
    }

    IEnumerator HurtRoutine()
    {
        yield return new WaitForSeconds(hurtDuration);

        isHurt = false;
    }

    private void OnKill() 
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
    }
}
