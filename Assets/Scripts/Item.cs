using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int ID;

    public Sprite sprite;


    public void Start()
    {
        if (ID <= 0)
        {
            Destroy(gameObject);
            Debug.Log("ID out of range");
        }

        else if (ID > 5)
        {
            Destroy(gameObject);
            Debug.Log("ID out of range");
        }
    }
}
