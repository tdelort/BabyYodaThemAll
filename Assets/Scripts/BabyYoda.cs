using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class BabyYoda : Player
{
    [SerializeField] private GameObject cosmeticSabre;

    // ACTION 1
    [SerializeField] private GameObject sabreAttackPrefab;
    [SerializeField] private Vector3 sabreOffset;

    // ACTION 2
    [SerializeField] private GameObject ondeObject;

    NetworkVariable<bool> isUsingAction1 = new NetworkVariable<bool>(false);
    NetworkVariable<bool> isUsingAction2 = new NetworkVariable<bool>(false);
    NetworkVariable<bool> hideSabre = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        cosmeticSabre.SetActive(true);
        hideSabre.OnValueChanged += OnHideSabre;
        Debug.Assert(cosmeticSabre != null);
    }

    private void OnHideSabre(bool before, bool after)
    {
        cosmeticSabre.SetActive(!after);
    }

    // ############ ACTION 1 ############
    // Competence 1 : Sabre throw
    protected override void InitAction1()
    {
        Debug.Assert(sabreAttackPrefab != null);
    }

    override protected void Action1()
    {
        Debug.Log("BabyYoda.Action1");
        if(isUsingAction1.Value)
            return;

        if(IsLocalPlayer)
        {
            Action1ServerRpc();
        }
    }

    [ServerRpc]
    public void Action1ServerRpc()
    {
        Debug.Log("BabyYoda.Action1ServerRpc");
        StartCoroutine(Action1Coroutine());
    }

    // Launch only in server
    protected virtual IEnumerator Action1Coroutine()
    {
        Debug.Log("BabyYoda.Action1Coroutine");
        isUsingAction1.Value = true;
        hideSabre.Value = true;


        GameObject sabreAttack = Instantiate(sabreAttackPrefab, Vector3.zero, transform.rotation);
        sabreAttack.GetComponent<NetworkObject>().Spawn();

        // For debugging
        // if(!NetworkManager.Singleton.IsHost)
        //    sabreAttack.GetComponent<NetworkObject>().NetworkHide(OwnerClientId);

        sabreAttack.transform.SetParent(transform);
        sabreAttack.transform.localPosition = sabreOffset;
        sabreAttack.GetComponent<Sabre>().Rotate();
        sabreAttack.transform.GetChild(0).GetComponent<Sabre>().Rotate();

        yield return new WaitForSeconds(1f);

        sabreAttack.SetActive(false);
        hideSabre.Value = false;

        // Add aditional delay to prevent spamming here
        yield return new WaitForSeconds(0.1f);

        sabreAttack.GetComponent<NetworkObject>().Despawn();
        Destroy(sabreAttack);

        isUsingAction1.Value = false;
    }

    // ############ ACTION 2 ############
    // Ultimate attack : Radial shockwave
    protected override void InitAction2()
    {
        Debug.Assert(ondeObject != null);
        ondeObject.SetActive(false);
    }

    override protected void Action2()
    {
        Debug.Log("BabyYoda.Action2");
        if(isUsingAction2.Value)
            return;

        if(IsLocalPlayer)
        {
            Action2ServerRpc();
        }
    }

    [ServerRpc]
    public void Action2ServerRpc()
    {
        Debug.Log("BabyYoda.Action1ServerRpc");
        StartCoroutine(Action2Coroutine());
    }

    [ClientRpc]
    public void SetActiveOndeClientRpc(bool active)
    {
        ondeObject.SetActive(active);
    }

    // Only in server
    private IEnumerator Action2Coroutine()
    {
        Debug.Log("BabyYoda.Action2Coroutine");
        isUsingAction2.Value = true;
        ondeObject.SetActive(true);
        SetActiveOndeClientRpc(true);

        yield return new WaitForSeconds(2.8f);

        ondeObject.SetActive(false);
        SetActiveOndeClientRpc(false);

        // Add aditional delay to prevent spamming here

        isUsingAction2.Value = false;
    }

    // ############ ACTION 3 ############
    // Competence 2 : Linear shockwave
    override protected void Action3()
    {
        Debug.Log("BabyYoda.Action3");
    }

    // ############ ACTION 4 ############
    // Casual attack : Sword slash
    override protected void Action4()
    {
        Debug.Log("BabyYoda.Action4");
    }
}
