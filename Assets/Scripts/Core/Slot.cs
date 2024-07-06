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
        //movementManager.MovementStopped += StopStopWatch;

        playButton.onClick.AddListener(OnRollPress);

        TargetSelector.RegisterSlot?.Invoke(this);
    }

    private void FixedUpdate()
    {
        movementManager.CheckMovement();
    }

    private void OnRollPress()
    {
        StartRoll();
        //StartStopWatch();
    }

    internal void SetTarget(SlotItemType type)
    {
        targetType = type;
    }

    private void StartRoll()
    {
        movementManager.StartRoll(ResultDelay, targetType);
    }

    /// <summary>
    /// commented section is for countdown.
    /// </summary>
    //[SerializeField] private TMP_Text timer;
    //private Stopwatch stopwatch = new Stopwatch();
    //internal void StartStopWatch()
    //{
    //    timer.text = "";
    //    stopwatch.Start();
    //}
    //
    //internal void StopStopWatch()
    //{
    //    TimeSpan time = stopwatch.Elapsed;
    //    time = time - TimeSpan.FromSeconds(ResultDelay * 0.2f);
    //    timer.text = time.ToString(@"ss\.fff");
    //    stopwatch.Reset();
    //}
    //

    private void OnDestroy()
    {
        //stopwatch.Stop();
        //stopwatch = null;
        //if(movementManager != null)
        //    movementManager.MovementStopped -= StopStopWatch;
        playButton.onClick.RemoveListener(OnRollPress);
        movementManager.Clear();
        setupManager.Clear();
        slotInfo.Clean();
        movementManager = null;
        setupManager = null;
        StopAllCoroutines();
    }
}
