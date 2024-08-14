using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{

    public Sprite charcterSprite;

    int characterNum = 0;

    public Texture[] characterList;

    public RawImage image;

    public CarController carController;

    public void LeftButtonclick()
    {
        if (characterNum == 0)
        {
            return;
        }
        else
        {
            characterNum--;
            UpdateCharacter();
        }
    }   
    
    public void RightButtonClick()
    {
        if (characterNum == 7)
        {
            return;
        }
        else
        {
            characterNum++;
            UpdateCharacter();
        }
    }

    public void UpdateCharacter()
    {
        image.texture = characterList[characterNum];
        carController.spriteRenderer.sprite = carController.carSprites[characterNum];
    }
}
