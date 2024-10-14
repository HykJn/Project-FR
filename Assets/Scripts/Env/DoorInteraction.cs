using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public bool col = false;

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
            player.GetComponent<PlayerController>()._flagInteractable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            col = false;
            player.GetComponent<PlayerController>()._flagInteractable = false;
        }
    }

    void Interaction()
    {
        if(col && Input.GetKeyDown(KeyCode.E))
        {
            from.SetActive(false);
            to.SetActive(true);
            player.GetComponent<RectTransform>().position = pos.position;
        }
    }
}
