using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookHP : MonoBehaviour

{
    [SerializeField] private int bookHP = 10;
    // Start is called before the first frame update
    void Start()
    {
        bookHP = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (bookHP == 0)
        {
            this.gameObject.SetActive(false);
        }
    }



    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "enemy" && bookHP > 0)
        {
            bookHP--;
            Debug.Log(bookHP);
        }
            
    }
}
