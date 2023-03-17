using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D m_Body { get; protected set; }
    public float range = 1000.0f;
    [SerializeField] 
    public float speed = 300;
    //[SerializeField]
    //protected GameObject m_ProjectileContactBehaviourPrefab;
    [SerializeField]
    protected List<GameObject> m_ProjectileContactBehaviourPrefabs;
    public WeaponBase OwningWeapon { get; set; }

    private string[] m_TagsToIgnore = { "Player","PlayerProjectile","Weapon" };
    public string[] TagsToIgnore { get { return m_TagsToIgnore; } }

    //allow contacts scripts to store data between hits
    public Dictionary<string, object> contactStorage { get; protected set; }

    //to prevent destruction on contact at least one script has to set its flag to false
    public Dictionary<string, bool> markForDestroyFlags { get; protected set; }

    public Vector3 lastVelocity;

    void Awake()
    {
        m_Body = GetComponent<Rigidbody2D>();
        contactStorage = new Dictionary<string, object>();
        markForDestroyFlags = new Dictionary<string, bool>();
    }

    void Update()
    {
        bool marked = true;
        foreach(var flag in markForDestroyFlags)
        {
            if(flag.Value == false)
            {
                marked = false;
                break;
            }
        }
        lastVelocity = m_Body.velocity;
        if (transform.position.magnitude > range || (marked && markForDestroyFlags.Count > 0))
        {
            Destroy(gameObject);
        }
    }

    public void Fire(Vector2 direction)
    {
        m_Body.AddForce(direction * speed);
    }

    //check if not owner
    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision with: "+ other.gameObject.tag);
        if (m_TagsToIgnore.Contains(other.gameObject.tag))
        {
            return;
        }
        else 
        {
            foreach(GameObject projectileContactBehaviourPrefab in m_ProjectileContactBehaviourPrefabs)
            {
                GameObject projectileContactBehaviour = Instantiate(projectileContactBehaviourPrefab, transform.position, Quaternion.identity);

                ProjectileContact contactBehaviour = projectileContactBehaviour.GetComponent<ProjectileContact>();
                contactBehaviour.lastVelocity = lastVelocity;
                contactBehaviour.OnContact(this, other);
            }
           
        }
    }

}   
