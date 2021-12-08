using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basique : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    private bool left = false;
    private bool right = false;

    private void Update()
    {
        if (left)
        {
            transform.Rotate(0, -360 * speed * Time.deltaTime, 0);
        }
        if (right)
        {
            transform.Rotate(0, 360* speed * Time.deltaTime, 0);
        }
    }

    public IEnumerator Attack()
    {
        left = true;
        yield return new WaitForSeconds(0.2f);
        left = false;
        right = true;
        yield return new WaitForSeconds(0.2f);
        right = false;
    }
}
