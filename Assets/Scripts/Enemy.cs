using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Enemy : MonoBehaviour
{

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
        targets = GameObject.FindObjectsOfType<Player>();
        agent.updatePosition = false;

        if (enemyType == 0) {
            moveName = "Jump";
            attackName = "Tongue";
            dieName = "Smashed";
        } else if (enemyType == 1) {
            moveName = "walk";
            attackName = "attack";
            dieName = "die";
        }
    }

    void Update()
    {
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
            }
        }
    }

    void OnAnimatorMove ()
    {
        // Update position to agent position
        transform.position = agent.nextPosition;
    }

    // Update is called once per frame
    /*void Update()
    {
        Debug.Log("Ecart de pos: " + (Math.Abs(agent.nextPosition.y - lastY)));
        if (targets.Length == 0) {
            targets = GameObject.FindObjectsOfType<Player>();
        }
        else {
            if (Math.Abs(agent.nextPosition.y - lastY) > 0.0001f) {
                agent.SetDestination(targets[0].transform.position);
                Debug.Log("Moving");
            } else {
                agent.isStopped = true;
                if (currentTime < waitingTime) {
                    currentTime += Time.deltaTime;
                    Debug.Log("Time remaining : " + (waitingTime - currentTime));
                } else {
                    agent.enabled = false;
                    body.isKinematic = false;
                    body.useGravity = true;
                    body.AddRelativeForce(new Vector3(0, 5f, 0), ForceMode.Impulse);
                    //agent.Warp(new Vector3(agent.nextPosition.x, agent.nextPosition.y+1, agent.nextPosition.z));
                    Debug.Log("Trying to jump");
                }
            }
        }
        lastY = agent.nextPosition.y;
    }*/

}
