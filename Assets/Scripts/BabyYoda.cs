using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class BabyYoda : Player
{
    [SerializeField] private GameObject cosmeticSabre;

    // ACTION 1
    [SerializeField] private GameObject sabreAttackObject;
    //[SerializeField] private Vector3 sabreOffset;

    // ACTION 2
    [SerializeField] private GameObject ondeObject;

    // ACTION 3
    [SerializeField] private GameObject pushObject;

    NetworkVariable<bool> isUsingAction1 = new NetworkVariable<bool>(false);
    NetworkVariable<bool> isUsingAction2 = new NetworkVariable<bool>(false);
    NetworkVariable<bool> isUsingAction3 = new NetworkVariable<bool>(false);
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
        Debug.Assert(sabreAttackObject != null);
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

    [ClientRpc]
    public void SetActiveSabreClientRpc(bool active)
    {
        Debug.Log("BabyYoda.SetActiveSabreClientRpc");
        sabreAttackObject.SetActive(active);
    }

    // Launch only in server
    protected virtual IEnumerator Action1Coroutine()
    {
        Debug.Log("BabyYoda.Action1Coroutine");
        isUsingAction1.Value = true;
        hideSabre.Value = true;
        sabreAttackObject.SetActive(true);
        SetActiveSabreClientRpc(true);

        yield return new WaitForSeconds(1f);

        sabreAttackObject.SetActive(false);
        SetActiveSabreClientRpc(false);
        hideSabre.Value = false;

        // Add aditional delay to prevent spamming here
        yield return new WaitForSeconds(0.1f);

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
        Debug.Log("BabyYoda.Action2ServerRpc");
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
    protected override void InitAction3()
    {
        Debug.Assert(pushObject != null);
        pushObject.SetActive(false);
    }

    override protected void Action3()
    {
        Debug.Log("BabyYoda.Action3");
        if(isUsingAction3.Value)
            return;

        if(IsLocalPlayer)
        {
            Action3ServerRpc();
        }
    }

    [ServerRpc]
    public void Action3ServerRpc()
    {
        Debug.Log("BabyYoda.Action3ServerRpc");
        StartCoroutine(Action3Coroutine());
    }

    [ClientRpc]
    public void SetActivePushClientRpc(bool active)
    {
        pushObject.SetActive(active);
    }

    // Only in server
    private IEnumerator Action3Coroutine()
    {
        Debug.Log("BabyYoda.Action3Coroutine");
        isUsingAction3.Value = true;
        pushObject.SetActive(true);
        SetActivePushClientRpc(true);

        yield return new WaitForSeconds(0.3f);

        pushObject.SetActive(false);
        SetActivePushClientRpc(false);

        // Add aditional delay to prevent spamming here

        isUsingAction3.Value = false;
    }

    // ############ ACTION 4 ############
    // Casual attack : Sword slash
    override protected void Action4()
    {
        Debug.Log("BabyYoda.Action4");
    }
}
