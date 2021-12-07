using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyDamage : NetworkBehaviour 
{

    [SerializeField] Material hitMat;

    private NetworkVariable<bool> highlighted = new NetworkVariable<bool>(false);
    private NetworkVariable<int> health = new NetworkVariable<int>(100);

    private Coroutine hlCoroutine;

    Material oldMat;
    private void Start()
    {
        oldMat = GetComponent<Renderer>().material;
        highlighted.OnValueChanged += OnHighlighted;
    }

    private void OnHighlighted(bool before, bool after)
    {
        Debug.LogError("Highlighted: " + after);
        if (after)
            GetComponent<Renderer>().material = hitMat;
        else
            GetComponent<Renderer>().material = oldMat;
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
                health.Value -= 10;
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
