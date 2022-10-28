using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUp : MonoBehaviour
{
    [Header("Set In Inspector: PowerUp")]
    // This is an unusual use of Vector2s. x holds a min value
    // and y a max value for a Random.Range() that will be called later

    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f; // Seconds the PowerUp exists
    public float fadeTime = 4f; // Seconds it will then fade

    [Header("Set Dynamically: PowerUp")]
    public WeaponType type; // The type of the PowerUp
    public GameObject cube; // Reference to the Cube Child
    public TMPro.TextMeshPro letter; // Reference to the Letter
    public Vector3 rotPerSecond; // Euler rotation speed
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    void Awake()
    {
        // Find the Cube reference
        cube = transform.Find("Cube").gameObject;

        // Find the TextMesh and other components
        letter = GetComponent<TMPro.TextMeshPro>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();

        // Set a random velocity
        Vector3 vel = Random.onUnitSphere; // Get Random xyz velocity
        // Random.onUnitSphere gives you a vector point that is somewhere on
        // the surface of the sphere with a radius of 1m around the origin

        vel.z = 0; // Flatten the vel to the XY plane
        vel.Normalize(); // Make vel length of 1m

        vel *= Random.Range(driftMinMax.x, driftMinMax.y); // Multiply length of vel by a random amount in driftMinMax
        rigid.velocity = vel;

        // Set the rotation of this GameObject to R:[0,0,0]
        transform.rotation = Quaternion.identity;

        // Quaternion.identity is equal to no rotation

        // Set up the rotPerSecond for the Cube child using rotMinMax x & y
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), 
                                   Random.Range(rotMinMax.x, rotMinMax.y), 
                                   Random.Range(rotMinMax.x, rotMinMax.y));
        birthTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Spin the Cube child every Update
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        // Gradually fade out the PowerUp over time
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime; // u goes from 0 to 1 over time

        // For lifeTime seconds, u will be <= 0. Then it will transition to
        // 1 over the course of fadeTime seconds

        // If u>=1, Destroy this.gameObject
        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }
        // Otherwise, use u to determine the alpha value of the Cube & Letter
        if (u > 0)
        {

            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }

        if (!bndCheck.isOnScreen)
        {
            // Once the PowerUp has left the screen,
            // Destroy this.gameObject
            Destroy(gameObject);
        }

    }

    public void SetType(WeaponType wt)
    {
        // Grab the weaponDefinition from Main
        WeaponDefinition def = Main.GetWeaponDefinition(wt);

        // Set the color of both the Cube & the Letter
        cubeRend.material.color = def.color;
        letter.color = def.color;

        letter.text = def.letter; // Set the letter that is shown
        type = wt; // Finally actually set the _type
    }

    public void AbsorbedBy(GameObject target)
    {
        // This function is called by the Hero class when a hero collects a PowerUp
        // We could tween into the target and shrink in size,
        // but for now, just destroy this.gameObject

        Destroy(this.gameObject);
    }
}
