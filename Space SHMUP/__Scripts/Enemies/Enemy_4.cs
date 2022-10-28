using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // <summary>
    // Enemy_4 will start offscreen and then pick a random point on the screen to
    // move to. Once it has arrived, it will pick another random point and
    // continue until the player has shot it down.
    // </summary>

    [System.Serializable]
    public class Part {
        public string name; // The name of this part
        public float health; // The amount of health this part has
        public string[] protectedBy; // The other parts that protect this
        [HideInInspector] // These two fields will not show in the Inspector
        public GameObject go; // A reference to the GameObject of this part
        [HideInInspector]
        public Material mat; // A reference to the Material to show damage
    }

public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts; // The array of ship Parts
    private Vector3 p0, p1;
    private float timeStart;
    private float duration = 4;

    void Start()
    {
        p0 = p1 = pos;

        InitMovement();

        // Cache GameObject and Material of each Part in parts
        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }

        // InvokeRepeating("CheckOffscreen", 0f, 2f);
    }

    void InitMovement()
    {
        p0 = p1;
        // Set p1 to a new Vector3 that is on screen
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);
        // Reset the time
        timeStart = Time.time;
    }

    public override void Move()
    {
        // This completely overrides Enemy.Move() with a linear interpolation
        float u = (Time.time - timeStart) / duration;
        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2); // Apply Ease Out easing to u
        pos = (1 - u) * p0 + u * p1; // Simple linear interpolation
    }

    Part FindPart(string n)
    {
        foreach (Part prt in parts)
        {
            if (prt.name == n)
            {
                return (prt);
            }
        }
        return (null);
    }

    Part FindPart(GameObject go)
    {
        foreach (Part prt in parts)
        {
            if (prt.go == go)
            {
                return (prt);
            }
        }
        return (null);
    }
    
    bool Destroyed(Part prt)
    {
        if (prt == null)
        {
            // This means it's not really part
            return (true);
        }
        // Returns whether the part is destroyed
        return (prt.health <= 0);
    }

    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                // If this Enemy is off screen, don't damage it.
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }
                // Hurt this Enemy
                // Get the GameObject that was hit
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                // Get the part of this ship that was hit
                Part prtHit = FindPart(goHit);
                // Check whether this part is still protected
                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        // If one of the protecting parts hasn't been destroyed...
                        if (!Destroyed(FindPart(s)))
                        {
                            // ...then don't damage this part yet
                            Destroy(other); // Destroy the ProjectileHero
                            return; // Return before damaging Enemy_4
                        }
                    }
                }
                // It's not protected, so make it take damage
                // Get the damage amount from the Projectile.type & Main.W_DEFS
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                // Show damage on the part
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    // Instead of destroying this enemy, disable the damaged part
                    prtHit.go.SetActive(false);
                }
                // Check to see if the whole ship is destroyed
                bool allDestroyed = true; // Assume it is destroyed
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt))
                    {
                        // If a part still exists...
                        allDestroyed = false; // ...change allDestroyed to false
                        break; // ...and break out of the foreach loop
                    }
                }
                if (allDestroyed)
                {
                    // Tell the Main singleton that this ship has been destroyed
                    Main.S.ShipDestroyed(this);
                    // Destroy this Enemy
                    Destroy(this.gameObject);
                }
                Destroy(other); // Destroy the ProjectileHero
                break;
        }
    }
    
}
