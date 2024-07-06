using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using JoyKit.Utility;

public class TargetSelector : MonoBehaviour
{
    internal static Action<Slot> RegisterSlot;

    private List<Slot> slots = new List<Slot>();
    private SlotItemType[] itemTypes;

    [SerializeField] private Button slot_Template;
    [SerializeField] private Button slotItem_Template;

    [SerializeField] private Transform slot_Parent;
    [SerializeField] private Transform slotitem_Parent;

    [SerializeField] private SlotItemInfo slotInfo;

    [SerializeField] private Transform uiBase;

    private ObjectPooler<Button> slotPooler;
    private ObjectPooler<Button> slotItemPooler;

    private void Awake()
    {
        RegisterSlot += Register;
        slotPooler = new ObjectPooler<Button>(slot_Template);
        slotItemPooler = new ObjectPooler<Button>(slotItem_Template);
    }

    private void Start()
    {
        itemTypes = slotInfo.GetSlotItemArray();
    }

    public void EnableUI()
    {
        uiBase.gameObject.SetActive(true);
        GenerateSlotMenu();
    }

    public void DisableUI()
    {
        uiBase.gameObject.SetActive(false);
        ClearSlotMenu();
        ClearSlotMenu();
    }

    private void Register(Slot slot)
    {
        if (slots.Contains(slot)) return;
        slots.Add(slot);
    }

    private void GenerateSlotMenu()
    {
        ClearSlotMenu();

        foreach (var slot in slots)
        {
            var button = GenerateButton(slotPooler, slot_Parent);
            button.onClick.AddListener(() => GenerateSlotItemsMenu(slot));
            button.GetComponentInChildren<TMP_Text>(true).text = slot.gameObject.name;
        }

        var button1 = GenerateButton(slotPooler, slot_Parent);
        button1.onClick.AddListener(() => GenerateSlotItemsMenu());
        button1.GetComponentInChildren<TMP_Text>(false).text = "All";
    }

    private Button GenerateButton(ObjectPooler<Button> pooler, Transform parent)
    {
        var button = pooler.Get();
        button.onClick.RemoveAllListeners();
        button.transform.SetParent(parent, false);
        return button;
    }

    private void ClearSlotMenu()
    {
        foreach (Transform child in slot_Parent)
        {
            slotPooler.Return(child.GetComponent<Button>());
        }
    }

    private void GenerateSlotItemsMenu(Slot slot = null)
    {
        ClearSlotItems();

        foreach (var itemType in itemTypes)
        {
            var button = GenerateButton(slotItemPooler, slotitem_Parent);
            button.GetComponentInChildren<SlotItemTSTemplate>().target.sprite = slotInfo.GetValue(itemType);
            AddActionToButton(button, itemType, slot);
        }

        var button2 = GenerateButton(slotItemPooler, slotitem_Parent);
        AddActionToButton(button2, SlotItemType.none, slot);
    }

    private void AddActionToButton(Button button, SlotItemType itemType, Slot slot)
    {
        if (slot != null)
        {
            button.onClick.AddListener(() => slot.SetTarget(itemType));
        }
        else
        {
            foreach (var slotHolder in slots)
            {
                button.onClick.AddListener(() => slotHolder.SetTarget(itemType));
            }
        }
    }

    private void ClearSlotItems()
    {
        foreach (Transform child in slotitem_Parent)
        {
            slotItemPooler.Return(child.GetComponent<Button>());
        }
    }

    private void OnDestroy()
    {
        RegisterSlot -= Register;
        slots.Clear();
        itemTypes = null;
        slotPooler.Clear();
        slotItemPooler.Clear();
    }
}

