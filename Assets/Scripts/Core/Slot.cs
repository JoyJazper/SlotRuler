using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private RectTransform slotPanel;
    [SerializeField] private SlotItem template;
    [SerializeField] private SlotItemInfo slotInfo;
    [SerializeField] private SlotItemType targetType;
    [SerializeField] private int resultDelayOrder = 0;
    [SerializeField] private int slotItemCount = 7;
    [SerializeField] private Button playButton;

    public int slotID;
    #region Instance Independent
    private static int slotCount = 0;
    private static bool[] Ready;
    private static bool IsAllSlotsReady
    {
        get
        {
            if (Ready == null || Ready.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < slotCount; i++)
            {
                if (!Ready[i])
                {
                    return false;
                }
            }
            return true;
        }

        set
        {
            for (int i = 0; i < slotCount; i++)
            {
                Ready[i] = value;
            }
        }
    }

    private static bool CheckIsReady()
    {
        slotCount++;
        bool isReady = IsAllSlotsReady;
        if (slotCount >= Ready.Length)
        {
            IsAllSlotsReady = false;
            slotCount = 0;
        }
        return isReady;
    }

    private static void SetIsReady()
    {
        slotCount++;
        if (slotCount >= Ready.Length)
        {
            IsAllSlotsReady = true;
            slotCount = 0;
        }
    }
    #endregion

    private SlotMovementManager movementManager;

    private SlotItem[] visibleItems;

    private void Awake()
    {
        slotID = slotCount;
        slotCount++;
    }

    private void Start()
    {
        if(Ready == null)
        {
            Ready = new bool[slotCount];
            slotCount = 0;
        }

        TargetSelector.RegisterSlot?.Invoke(this);
        
        SetupMovementManager();
        playButton.onClick.AddListener(OnRollPress);
    }

    private void SetupMovementManager()
    {
        int lasttop = slotItemCount - 1;
        SlotSetupManager setupManager = new SlotSetupManager(slotPanel, template, slotInfo, slotItemCount);
        (visibleItems, lasttop) = setupManager.SetupPlayGround();
        movementManager = new SlotMovementManager(resultDelayOrder, lasttop, ref slotInfo, ref visibleItems);
        movementManager.MovementStopped += SetIsReady;
        setupManager.Clear();
        SetIsReady();
    }

    private void FixedUpdate()
    {
        movementManager.CheckMovement();
    }

    private void OnRollPress()
    {
        if (CheckIsReady())
        {
            movementManager.StartRoll(targetType);
        }
        //StartStopWatch();
    }

    internal void SetTarget(SlotItemType type)
    {
        targetType = type;
    }


    private void OnDestroy()
    {
        movementManager.MovementStopped -= SetIsReady;
        playButton.onClick.RemoveListener(OnRollPress);
        movementManager.Clear();
        slotInfo.Clean();
        Ready = null;
        movementManager = null;
        StopAllCoroutines();
    }
}
