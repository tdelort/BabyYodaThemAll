using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class BabyYoda : Player
{
    [SerializeField] private GameObject sabreAttackPrefab;
    [SerializeField] private GameObject cosmeticSabre;
    [SerializeField] private Vector3 sabreOffset;

    bool isUsingAction1 = false;

    // ############ ACTION 1 ############
    protected override void InitAction1()
    {
        Debug.Assert(sabreAttackPrefab != null);
        Debug.Assert(cosmeticSabre != null);

        cosmeticSabre.SetActive(true);
    }
    override protected void Action1()
    {
        Debug.Log("BabyYoda.Action1");
        if(isUsingAction1)
            return;

        if(IsLocalPlayer)
        {
            Action1ServerRpc();
            // Here we could start an animation that mimicks the sabre attack
            // Because else it would not be synchronised
            // StartCoroutine(Action1Coroutine());
        }
    }

    [ServerRpc]
    public void Action1ServerRpc()
    {
        Debug.Log("BabyYoda.Action1ServerRpc");
        StartCoroutine(Action1Coroutine());
    }

    protected virtual IEnumerator Action1Coroutine()
    {
        Debug.Log("BabyYoda.Action1Coroutine");
        isUsingAction1 = true;

        GameObject sabreAttack = Instantiate(sabreAttackPrefab, Vector3.zero, transform.rotation);

        if(NetworkManager.Singleton.IsServer)
        {
            sabreAttack.GetComponent<NetworkObject>().Spawn();
            //sabreAttack.GetComponent<NetworkObject>().NetworkHide(OwnerClientId);
        }

        sabreAttack.transform.SetParent(transform);
        sabreAttack.transform.localPosition = sabreOffset;
        sabreAttack.GetComponent<Sabre>().Rotate();
        sabreAttack.transform.GetChild(0).GetComponent<Sabre>().Rotate();

        cosmeticSabre.SetActive(false);
        yield return new WaitForSeconds(1f);
        cosmeticSabre.SetActive(true);

        sabreAttack.SetActive(false);

        // Add aditional delay to prevent spamming here
        yield return new WaitForSeconds(0.1f);

        if(NetworkManager.Singleton.IsServer)
        {
            sabreAttack.GetComponent<NetworkObject>().Despawn();
        }
        Destroy(sabreAttack);

        isUsingAction1 = false;
    }

    // ############ ACTION 2 ############
    override protected void Action2()
    {
        Debug.Log("BabyYoda.Action2");
    }

    override protected void Action3()
    {
        Debug.Log("BabyYoda.Action3");
    }
}
