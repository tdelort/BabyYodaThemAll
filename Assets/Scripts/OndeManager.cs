using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OndeManager : MonoBehaviour
{
    [SerializeField] private Onde onde1 = null;
    [SerializeField] private Onde onde2 = null;
    [SerializeField] private Onde onde3 = null;

    private void OnEnable()
    {
        onde1.gameObject.SetActive(false);
        onde2.gameObject.SetActive(false);
        onde3.gameObject.SetActive(false);
        StartCoroutine(LaunchWaves());
    }

    private IEnumerator LaunchWaves()
    {
        Debug.Log("Onde1");
        onde1.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        Debug.Log("Onde2");
        onde2.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        Debug.Log("Onde3");
        onde1.gameObject.SetActive(false);
        onde3.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        onde2.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.7f);
        onde3.gameObject.SetActive(false);
    }
}
