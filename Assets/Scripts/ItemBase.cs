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
            switch(ID){
                case 1:
                    carController.ApplySpeedBoost(8f, .3f);
                    ID = 0;
                    break;
                case 2:
                    carController.ApplySpeedBoost(1.3f, 12f);
                    ID = 0;
                    break;
                default:
                    ID = 0;
                    break;
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
