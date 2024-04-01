using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    public GameObject bullets;
    public Transform anchor;
    public float shootCD;
    public bool isConstantShooting = false;
    protected bool canShoot = true;

 
    public void Shoot()
    {
        if (!canShoot) return;
        canShoot = false;
        StartCoroutine(shootCoolDown());

        if(bullets != null)
        {
            Instantiate(bullets, anchor.position, anchor.rotation);
        }

        Debug.Log("shoot");
    }

    private void Update()
    {
        if(isConstantShooting) { Shoot(); }
    }

    IEnumerator shootCoolDown()
    {
        yield return new WaitForSeconds(shootCD);
        canShoot = true;
        yield return null;
    }

    public void ConstantShoot(bool mode)
    {
        isConstantShooting = mode;
    }
}
