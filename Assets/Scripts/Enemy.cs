using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System;
using Unity.Netcode;

public class Enemy : NetworkBehaviour
{   
    // Highlight and taking damages
    [SerializeField] Material hitMaterial;
    Material originalMaterial;
    private NetworkVariable<bool> highlighted = new NetworkVariable<bool>(false);
    private NetworkVariable<int> health = new NetworkVariable<int>(2);
    private Coroutine hlCoroutine;
    [SerializeField] Renderer bodyRenderer;

    // Movments and animations
    public NavMeshAgent agent;
    public Animator animator;
    public int enemyType;

    private Player[] targets;
    private Vector2 smoothDeltaPosition = Vector2.zero;
    private Vector2 velocity = Vector2.zero;
    private String moveName;
    private String attackName;
    private String dieName;
    

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(bodyRenderer != null);
        Debug.Assert(hitMaterial != null);

        if(NetworkManager.Singleton.IsServer)
        {
            targets = GameObject.FindObjectsOfType<Player>();
            agent.updatePosition = false;

            if (enemyType == 0) {
                health.Value = 2;
                moveName = "Jump";
                attackName = "Tongue";
                dieName = "Smashed";
            } else if (enemyType == 1) {
                health.Value = 5;
                moveName = "walk";
                attackName = "attack";
                dieName = "die";
            }
        }

        originalMaterial = bodyRenderer.material;
        highlighted.OnValueChanged += OnHighlighted;
    }

    void Update()
    {
        // Server has auth on ennemies
        if(NetworkManager.Singleton.IsServer)
        {
            // ###### STATS ######

            if(health.Value <= 0)
            {
                animator.SetTrigger(dieName);
                GetComponent<NetworkObject>().Despawn();
                gameObject.SetActive(false);
                return;
            }

            // ###### MOVEMENTS ######
            if (targets.Length == 0) {
                targets = GameObject.FindObjectsOfType<Player>();
            }
            else {
                agent.destination = targets[0].transform.position;

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
                    animator.Play(attackName, -1);
                    // HANDLE ATTACK HERE
                }
            }
        }
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
            {
                health.Value--;
                if(hlCoroutine != null)
                    StopCoroutine(hlCoroutine);
                StartCoroutine(HighLight()); 
            }
        }
    }

    private IEnumerator HighLight()
    {
        highlighted.Value = true;
        yield return new WaitForSeconds(0.1f);
        highlighted.Value = false;
    }

}