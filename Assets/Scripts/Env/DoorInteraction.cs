using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    bool col = false;

    public GameObject from;
    public GameObject to;
    public Transform pos;

    GameObject player;

    void Update()
    {
        Interaction();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
            col = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) col = false;
    }

    void Interaction()
    {
        if(col && Input.GetKeyDown(KeyCode.E))
        {
            from.SetActive(false);
            to.SetActive(true);
            player.transform.position = pos.position;
        }
    }
}