using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SlotItem : MonoBehaviour, ISlottable
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    
    public float Height
    {
        get
        {
            if(rectTransform != null)
                return rectTransform.rect.height;
            else
            {
                rectTransform = GetComponent<RectTransform>();
                return rectTransform.rect.height;
            }
        }
    }

    private RectTransform rectTransform;
    private SlotItemType slotType; 
    private Vector2 tempPosition;

    public SlotItemType SlotType
    {
        get
        {
            return slotType;
        }

        private set { }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        tempPosition.x = rectTransform.anchoredPosition.x;
    }

    public void SetPosition(float yPosition)
    {
        tempPosition.y = yPosition;
        rectTransform.anchoredPosition = tempPosition;
    }

    public void MoveDown(float yCoef)
    {
        tempPosition.y = rectTransform.anchoredPosition.y - yCoef;
        rectTransform.anchoredPosition = tempPosition;
    }

    public float GetPosition()
    {
        return rectTransform.anchoredPosition.y;
    }

    public void SetCharacter(SlotItemType type, Sprite sprite)
    {
        slotType = type;
        image.sprite = sprite;
    }
}

public interface ISlottable
{
    void SetCharacter(SlotItemType type, Sprite sprite);
    void SetPosition(float yPosition);
}

public enum SlotItemType
{
    seven,
    spade,
    gem,
    heart,
    club,
    bar,
    bar2,
    bar3,
    cap,
    cash,
    bell,
    coin,
    coinM,
    ccoin,
    ccoinM,
    clove,
    cup,
    berry,
    melon,
    cherry,
    horseShoe
}
