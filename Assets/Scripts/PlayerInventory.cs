using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public int manaCount { get; private set; }

    public void manaPickup()
    {
        manaCount++;
    }
}
