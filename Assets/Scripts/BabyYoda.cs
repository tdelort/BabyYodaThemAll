using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class BabyYoda : Player
{
    [SerializeField] private GameObject sabreAttackPrefab;
    [SerializeField] private GameObject cosmeticSabre;
    [SerializeField] private Vector3 sabreOffset;

    NetworkVariable<bool> isUsingAction1 = new NetworkVariable<bool>(false);

    // ############ ACTION 1 ############
    protected override void InitAction1()
    {
        Debug.Assert(sabreAttackPrefab != null);
        Debug.Assert(cosmeticSabre != null);

        cosmeticSabre.SetActive(true);
        isUsingAction1.OnValueChanged += OnUsingAction1;
    }

    private void OnUsingAction1(bool before, bool after)
    {
        cosmeticSabre.SetActive(!after);
    }

    override protected void Action1()
    {
        Debug.Log("BabyYoda.Action1");
        if(isUsingAction1.Value)
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
        isUsingAction1.Value = true;

        GameObject sabreAttack = Instantiate(sabreAttackPrefab, Vector3.zero, transform.rotation);

        if(NetworkManager.Singleton.IsServer)
        {
            sabreAttack.GetComponent<NetworkObject>().Spawn();

            // For debugging
            // if(!NetworkManager.Singleton.IsHost)
            //    sabreAttack.GetComponent<NetworkObject>().NetworkHide(OwnerClientId);
        }

        sabreAttack.transform.SetParent(transform);
        sabreAttack.transform.localPosition = sabreOffset;
        sabreAttack.GetComponent<Sabre>().Rotate();
        sabreAttack.transform.GetChild(0).GetComponent<Sabre>().Rotate();

        yield return new WaitForSeconds(1f);

        sabreAttack.SetActive(false);

        // Add aditional delay to prevent spamming here
        yield return new WaitForSeconds(0.1f);

        if(NetworkManager.Singleton.IsServer)
        {
            sabreAttack.GetComponent<NetworkObject>().Despawn();
        }
        Destroy(sabreAttack);

        isUsingAction1.Value = false;
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
