using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public int ID { get; set; }

    public string[] name;

    public CarController carController;

    // Start is called before the first frame update
    void Start()
    {
        carController = FindAnyObjectByType<CarController>();
    }

    private void OnEnable()
    {
        CarController.OnUseItem += UseItem;
        CarController.OnGetItem += GetItem;
    }

    private void OnDisable()
    {
        CarController.OnUseItem -= UseItem;
        CarController.OnGetItem -= GetItem;
    }

    void UseItem()
    {
        if (ID != 0)
        {

            Debug.Log("Use Item Called.");
            if (ID == 1)
            {
                carController.ApplySpeedBoost();
                ID = 0;
            }
        }
        carController.UpdateItem();
    }

    void GetItem()
    {
        Debug.Log("GetItem Called");
        carController.UpdateItem();

    }
}
