using System.Collections;
using UnityEngine;
using DG.Tweening;
using JoyKit.Utility;
using System.Collections.Generic;
using System.Linq;

public class SlotMovementManager
{
    #region Fix Data
    private const float moveDownSpeed = 100f;
    private const float topSpeedCoef = 1f;
    private const float totalTime = 0.8f;
    private const float rollInitalPull = -0.5f;
    private const float delayTime = 0.2f;
    private const float resetDelay = 0.1f;
    private const float beforeEndHeight = 500f;

    private readonly SlotItemInfo info;
    private readonly SlotItem[] visibleItems;
    private readonly List<SlotItemType> itemTypes = new List<SlotItemType>();
    private readonly float checkEndHeight, itemHeight, resultDelay;
    #endregion

    #region Variables
    private float moveDownCoef = 1f;
    private bool shouldMove = false, outputSet = false;
    private int lastTopIndex = -1, targetItemIndex = -1;
    private SlotItemType targetItemType = SlotItemType.none;
    private List<int> reuseItemIndex = new List<int>();
    #endregion

    #region Setup
    public SlotMovementManager(int delayOrder, int topItemIndex, ref SlotItemInfo info, ref SlotItem[] visibleItems)
    {
        lastTopIndex = topItemIndex;
        this.visibleItems = visibleItems;
        this.info = info;
        itemTypes = info.GetSlotItemList();
        checkEndHeight = CalculateDepthBarrier();
        itemHeight = visibleItems[0].Height;
        resultDelay = delayOrder;
    }

    private float CalculateDepthBarrier()
    {
        if(visibleItems != null && visibleItems.Count() > 0)
            return ((visibleItems.Count() - 1) / 2 * -1) * visibleItems[0].Height;
        else 
            return 300f; // assumption
    }

    private void GenerateTarget()
    {
        targetItemIndex = UnityEngine.Random.Range(0, visibleItems.Length);
        targetItemType = visibleItems[targetItemIndex].SlotType;
        //CoroutineStarter.Instance.DED(" Random Target : " + targetItemType.ToString());
    }

    private void SetMoveVariables()
    {
        moveDownCoef = rollInitalPull;
        shouldMove = true;
    }

    private void SetStopVariables()
    {
        outputSet = false;
        shouldMove = false;
        targetItemIndex = -1;
    }

    public void Clear()
    {
        itemTypes.Clear();
    }

    #endregion

    #region Main Roll
    public void CheckMovement()
    {
        if (shouldMove)
        {
            Move();
        }
    }

    public void Move()
    {
        float rollSpeed = moveDownSpeed * moveDownCoef;
        
        for (int i = 0; i < visibleItems.Length; i++)
        {
            if (visibleItems[i].GetPosition() <= checkEndHeight)
            {
                reuseItemIndex.Add(i);
            }
            else
            {
                visibleItems[i].MoveDown(rollSpeed);
            }
        }
        if(reuseItemIndex.Count > 0)
        {
            ReuseItem(reuseItemIndex);
        }

    }

    private void MoveAllitems(float distance)
    {
        foreach (var item in visibleItems)
        {
            item.MoveDown(distance);
        }
    }

    private void ReuseItem(List<int> indexes)
    {
        foreach (var index in indexes)
        {
            if (targetItemType == visibleItems[index].SlotType)
            {
                targetItemIndex = -1;
            }

            float newYPosition = visibleItems[lastTopIndex].GetPosition() + itemHeight;
            visibleItems[index].SetPosition(newYPosition);

            int lastIndex = itemTypes.IndexOf(visibleItems[lastTopIndex].SlotType) + 1;
            if (lastIndex >= itemTypes.Count)
            {
                lastIndex = 0;
            }
            else if (lastIndex < 0)
            {
                lastIndex = 0;
            }

            lastTopIndex = index;

            SlotItemType newType = itemTypes[lastIndex];

            if (newType == targetItemType)
            {
                targetItemIndex = index;
            }
            visibleItems[index].SetCharacter(newType, info.GetValue(newType));
        }
        reuseItemIndex.Clear();
    }

    #endregion

    #region Roll State Management

    #region Start Roll
    public void StartRoll(SlotItemType targettype = SlotItemType.none)
    {
        if (shouldMove) return;

        SetMoveVariables();

        if (targettype != SlotItemType.none)
        {
            targetItemType = targettype;
        }
        else
        {
            GenerateTarget();
        }

        DOTween.To(() => moveDownCoef, x => moveDownCoef = x, topSpeedCoef, totalTime).OnComplete(() => CoroutineStarter.Instance.StartCoroutine(StopRoll()));
    }

    #endregion

    #region End Roll
    /// <summary>
    /// This controls the sequence of stopping the object.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StopRoll()
    {
        yield return new WaitForSeconds(resultDelay * delayTime);

        GetTargetOnTop();

        // setting the target at 500 distance
        while (true)
        {
            if (targetItemIndex >= 0)
            {
                float distance = visibleItems[targetItemIndex].GetPosition();
                SetResult(distance);
                if (Mathf.Approximately(distance, 0))
                    break;
            }
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(((2 - resultDelay) * delayTime) + resetDelay);
        SetStopVariables();
    }

    /// <summary>
    /// we calculate the items to generate to get the desired result on top.
    /// </summary>
    /// <returns></returns>
    private int CalculateIndexDistance()
    {
        int index1 = -1, index2 = -1;
        for (int i = 0; i < itemTypes.Count; i++)
        {
            if (itemTypes[i] == visibleItems[lastTopIndex].SlotType)
            {
                index1 = i;
            }
            if (itemTypes[i] == targetItemType)
            {
                index2 = i;
            }
            if (index1 != -1 && index2 != -1)
            {
                break;
            }
        }
        return (index2 >= index1) ? (index2 - index1) : (itemTypes.Count - (index1 - index2));
    }

    /// <summary>
    /// Move the objects required to get the desired target on top.
    /// </summary>
    private void GetTargetOnTop()
    {
        int IndexDistance = CalculateIndexDistance();

        float totalDistance = itemHeight * IndexDistance;
        MoveAllitems(totalDistance);
    }

    /// <summary>
    /// We set the target at a distance of "beforeEndHeight" above the final position and let it flow.
    /// For realism in slow speed animations.
    /// It still shifts but less obvious.
    /// - returns true if its zero
    /// </summary>
    /// <param name="distance">current distance from 0 to target</param>
    /// <returns></returns>
    private bool SetResult(float distance)
    {
        if (distance < 0)
        {
            return false;
        }

        if (distance < itemHeight)
        {
            moveDownCoef = 0f;
            float newPosition = visibleItems[targetItemIndex].GetPosition();
            MoveAllitems(newPosition);
            //MovementStopped?.Invoke();
            return true;
        }

        if (distance > itemHeight && !outputSet)
        {
            outputSet = true;
            float newPosition = visibleItems[targetItemIndex].GetPosition();
            MoveAllitems(newPosition - beforeEndHeight);
        }

        return false;
    }

    #endregion

    #endregion

}
