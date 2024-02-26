using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullets : BulletBehavior {
    public float life = 2;


    private void Start() {
        StartCoroutine(SelfDestruction());
    }

    private void Update() {
        transform.position +=(transform.forward * speed * Time.deltaTime);
    }

    IEnumerator SelfDestruction() {
        yield return new WaitForSeconds(life);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<EnemyMovement>() != null) {
            other.GetComponent<EnemyMovement>().TakeDamage();
            Destroy(gameObject);
        }
    }
}
