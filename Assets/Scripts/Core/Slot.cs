using System;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private RectTransform slotPanel;
    [SerializeField] private SlotItem template;
    [SerializeField] private SlotItemInfo slotInfo;
    [SerializeField] private SlotItemType targetType;
    [SerializeField] private float ResultDelay = 0f;
    [SerializeField] private int slotItemCount = 7;
    [SerializeField] private Button playButton;

    private SlotSetupManager setupManager;
    private SlotMovementManager movementManager;

    private SlotItem[] visibleItems;

    private void Start()
    {
        int lasttop = slotItemCount - 1;
        setupManager = new SlotSetupManager(slotPanel, template, slotInfo, slotItemCount);
        (visibleItems, lasttop) =  setupManager.SetupPlayGround();
        movementManager = new SlotMovementManager(slotPanel, lasttop, ref slotInfo, ref visibleItems);

        playButton.onClick.AddListener(OnRollPress);
    }

    private void FixedUpdate()
    {
        movementManager.CheckMovement();
    }

    private void OnRollPress()
    {
        StartRoll();
    }

    private void StartRoll()
    {
        movementManager.StartRoll(ResultDelay, targetType);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnRollPress);
        movementManager.Clear();
        setupManager.Clear();
        slotInfo.Clean();
        movementManager = null;
        setupManager = null;
        StopAllCoroutines();
    }
}
