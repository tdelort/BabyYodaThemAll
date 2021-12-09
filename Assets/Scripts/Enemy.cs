using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using Unity.Netcode;

public class Enemy : NetworkBehaviour
{   
    // Highlight and taking damages
    [SerializeField] Material hitMaterial;
    [SerializeField] GameObject lootPrefab;
    Material originalMaterial;
    private NetworkVariable<bool> highlighted = new NetworkVariable<bool>(false);
    private NetworkVariable<int> health = new NetworkVariable<int>(2);
    private int maxHealth = 2;
    private Coroutine hlCoroutine;
    [SerializeField] Renderer bodyRenderer;
    [SerializeField] Slider healthBar;
    bool isAttacking = false;

    // Movments and animations
    public NavMeshAgent agent;
    public Animator animator;
    public int enemyType;

    private Player[] targets;
    private Player target;
    private Vector2 smoothDeltaPosition = Vector2.zero;
    private Vector2 velocity = Vector2.zero;
    private String enemyName;
    private String moveName;
    private String attackName;
    private String dieName;
    

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        Debug.Assert(bodyRenderer != null);
        Debug.Assert(hitMaterial != null);

        if(NetworkManager.Singleton.IsServer)
        {
            targets = GameObject.FindObjectsOfType<Player>();
            agent.updatePosition = false;
        }

        if (enemyType == 0) {
            if(NetworkManager.Singleton.IsServer)
                health.Value = 2;
            maxHealth = 2;
            enemyName = "Frog";
            moveName = "Jump";
            attackName = "Tongue";
            dieName = "Smashed";
        } else if (enemyType == 1) {
            if(NetworkManager.Singleton.IsServer)
                health.Value = 5;
            maxHealth = 5;
            enemyName = "Spider";
            moveName = "walk";
            attackName = "attack";
            dieName = "die";
        }

        healthBar.value = 1;
        health.OnValueChanged += OnHealth;
        originalMaterial = bodyRenderer.material;
        highlighted.OnValueChanged += OnHighlighted;
    }

    private void OnHealth(int before, int after)
    {
        if(health.Value <= 0)
        {
            if(NetworkManager.Singleton.IsServer)
            {
                Vector3 pos = transform.position;
                var loot = Instantiate(lootPrefab, pos, Quaternion.identity);
                loot.GetComponent<NetworkObject>().Spawn();
                
                GetComponent<NetworkObject>().Despawn();
            }
            gameObject.SetActive(false);
        }

        healthBar.value = (float)health.Value / (float)maxHealth;
    }

    void Update()
    {
        // Server has auth on ennemies
        if(NetworkManager.Singleton.IsServer)
        {

            // ###### MOVEMENTS ######
            targets = GameObject.FindObjectsOfType<Player>();
            if(targets.Length <= 0)
                return;

            target = targets[0];
            float smallest = Vector3.Distance(transform.position, target.transform.position);
            foreach (var currentTarget in targets) {
                float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
                if (dist < smallest) {
                    smallest = dist;
                    target = currentTarget;
                }
            }

            agent.destination = target.transform.position;
            
            Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

            // Map 'worldDeltaPosition' to local space
            float dx = Vector3.Dot (transform.right, worldDeltaPosition);
            float dy = Vector3.Dot (transform.forward, worldDeltaPosition);
            Vector2 deltaPosition = new Vector2 (dx, dy);

            // Low-pass filter the deltaMove
            float smooth = Mathf.Min(1.0f, Time.deltaTime/0.15f);
            smoothDeltaPosition = Vector2.Lerp (smoothDeltaPosition, deltaPosition, smooth);

            // Update velocity if time advances
            if (Time.deltaTime > 1e-5f) {
                velocity = smoothDeltaPosition / Time.deltaTime;
            }

            if (agent.remainingDistance > agent.stoppingDistance) {
                animator.Play(moveName, -1);
                //animator.SetFloat ("velx", velocity.x);
                //animator.SetFloat ("vely", velocity.y);
            } else {
                if(!isAttacking)
                {
                    animator.Play(attackName, -1);
                    // HANDLE ATTACK HERE
                    StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.2f);
        Physics.SphereCast(transform.position, 1f, transform.forward, out RaycastHit hit, 1.0f);
        if(hit.collider != null)
        {
            if(hit.collider.gameObject.tag == "Player")
            {
                hit.collider.gameObject.GetComponent<Player>().TakeDamage(1);
            }
        }

        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    void OnAnimatorMove ()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            // Update position to agent position
            transform.position = agent.nextPosition;
        }
    }

    private void OnHighlighted(bool before, bool after)
    {
        //Debug.LogError("Highlighted: " + after);
        if (after)
            bodyRenderer.material = hitMaterial;
        else
            bodyRenderer.material = originalMaterial;
    }

    public void OnTriggerEnter(Collider other)
    {
        // Gestion des collisions entre les attaques et les ennemis
        // Dans le serveur pour éviter de faire des collisions 
        // sur des ennemis qui sont détruits ?
        if(NetworkManager.Singleton.IsServer)
        {
            if (other.tag == "PlayerAttack")
                health.Value--;
            else if(other.tag == "SabreAttack")
                health.Value -= 2;
            else if(other.tag == "PushAttack")
                health.Value -= 2;
            else if(other.tag == "OndeAttack")
                health.Value -= 5;
            else
                return;

            if(health.Value <= 0)
            {
                other.gameObject.GetComponentInParent<Player>().AddKill();
                return;
            }

            if(hlCoroutine != null)
                StopCoroutine(hlCoroutine);
            StartCoroutine(HighLight()); 
        }
    }

    private IEnumerator HighLight()
    {
        highlighted.Value = true;
        yield return new WaitForSeconds(0.1f);
        highlighted.Value = false;
    }

}