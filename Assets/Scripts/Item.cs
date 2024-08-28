using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int ID;

    public SpriteRenderer spriteRenderer;

    public Sprite sprite;


    public void Start()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        this.gameObject.GetComponent<SpriteRenderer>().enabled = true;

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


    public void UseItem()
    {
        spriteRenderer.enabled = false;
        StartCoroutine(ReEnable());
    }


    private IEnumerator ReEnable()
    {
        yield return new WaitForSeconds(5);
        spriteRenderer.enabled = true;
    }
}
