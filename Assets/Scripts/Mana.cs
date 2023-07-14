using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
        Debug.Log("working");
        if (playerInventory != null) {
            
            playerInventory.manaPickup();
            gameObject.SetActive(false);
        }
    }
}
