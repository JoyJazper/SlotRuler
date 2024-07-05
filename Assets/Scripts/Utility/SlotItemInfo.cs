using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotItemInfo", menuName = "ScriptableObjects/SlotItemInfo", order = 1)]
public class SlotItemInfo : ScriptableObject
{
    [System.Serializable]
    public class SlotItemData
    {
        public SlotItemType key;
        public Sprite value;
    }

    public SlotItemData[] slotItems;

    Dictionary<SlotItemType, Sprite> slotItemDictionary;

    private void OnEnable()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        slotItemDictionary = new Dictionary<SlotItemType, Sprite>();
        foreach (SlotItemData item in slotItems)
        {
            if (!slotItemDictionary.ContainsKey(item.key))
            {
                slotItemDictionary.Add(item.key, item.value);
            }
        }
    }

    public Sprite GetValue(SlotItemType key)
    {
        if (slotItemDictionary == null)
        {
            InitializeDictionary();
        }

        if (slotItemDictionary.ContainsKey(key))
        {
            return slotItemDictionary[key];
        }
        else
        {
            Debug.LogWarning($"Key '{key}' not found in SlotItemInfo.");
            return null;
        }
    }

    public List<SlotItemType> GetSlotItemList()
    {
        if (slotItemDictionary == null)
        {
            InitializeDictionary();
        }

        return slotItemDictionary.Keys.ToList();
    }
}
