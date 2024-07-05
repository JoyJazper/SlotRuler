using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class SlotSetupManager
{
    private RectTransform slotPanel;
    private SlotItem template;
    private SlotItemInfo slotInfo;
    private List<SlotItemType> items;
    private int slotItemCount;
    public SlotSetupManager(RectTransform slotPanel, SlotItem template, SlotItemInfo slotInfo, int slotItemCount)
    {
        this.slotPanel = slotPanel;
        this.template = template;
        this.slotInfo = slotInfo;
        this.slotItemCount = slotItemCount;
    }

    public (SlotItem[], int) SetupPlayGround()
    {
        SlotItem[] visibleItems = new SlotItem[slotItemCount];
        float[] setPositions = new float[slotItemCount];
        items = slotInfo.GetSlotItemList();
        int middle = 3;

        setPositions[middle] = 0.0f;
        visibleItems[middle] = GenerateItem(items[middle], setPositions[middle], middle);

        for (int i = 1; i <= 3; i++)
        {
            setPositions[middle + i] = setPositions[middle] + i * template.Height;
            visibleItems[middle + i] = GenerateItem(items[(middle + i) % items.Count], setPositions[middle + i], middle + i);

            setPositions[middle - i] = setPositions[middle] - i * template.Height;
            visibleItems[middle - i] = GenerateItem(items[(middle - i + items.Count) % items.Count], setPositions[middle - i], middle - i);
        }
        return (visibleItems, visibleItems.Count() - 1);
    }

    private SlotItem GenerateItem(SlotItemType type, float yPosition, int index)
    {
        SlotItem newItem = GameObject.Instantiate(template.gameObject, slotPanel).GetComponent<SlotItem>();
        newItem.gameObject.name = "Slot " + index.ToString();
        newItem.SetCharacter(type, slotInfo.GetValue(type));
        newItem.SetPosition(yPosition);
        return newItem;
    }

    public void Clear()
    {
        items.Clear();
        items = null;
    }
}
