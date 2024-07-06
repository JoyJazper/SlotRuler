using System.Collections;
using UnityEngine;
using DG.Tweening;
using JoyKit.Utility;
using System.Collections.Generic;

public class SlotMovementManager
{
    private readonly float moveDownSpeed = 100f;
    private readonly float topSpeedCoef = 1f;
    private readonly float totalTime = 1.5f;
    private readonly float rollInitalPull = -0.5f;

    private SlotItemInfo info;

    private float moveDownCoef = 1f, checkEndHeight = 0f, itemHeight = 200f, resultDelay = 0f;
    private bool shouldMove = false, fixedOutput = false;
    private int lastTopIndex = -1, targetItemIndex = -1;
    private SlotItemType targetItemType = SlotItemType.none;
    private SlotItem[] visibleItems;
    private List<SlotItemType> itemTypes = new List<SlotItemType>();

    public SlotMovementManager(int topItemIndex, ref SlotItemInfo info, ref SlotItem[] visibleItems)
    {
        lastTopIndex = topItemIndex;
        this.visibleItems = visibleItems;
        this.info = info;
        itemTypes = info.GetSlotItemList();
        checkEndHeight = -3f * visibleItems[0].Height;
        itemHeight = visibleItems[0].Height;
    }

    private void GenerateTarget()
    {
        targetItemIndex = UnityEngine.Random.Range(0, visibleItems.Length);
        targetItemType = visibleItems[targetItemIndex].SlotType;
        //CoroutineStarter.Instance.DED(" Random Target : " + targetItemType.ToString());
    }

    public void CheckMovement()
    {
        if (shouldMove)
        {
            Move();
        }
    }

    List<int> reuseItemIndex = new List<int>();

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

    public void StartRoll(float delay = 0f, SlotItemType targettype = SlotItemType.none)
    {
        if (shouldMove) return;

        SetMoveState(delay);

        if (targettype != SlotItemType.none)
        {
            targetItemType = targettype;
        }
        else
        {
            GenerateTarget();
        }

        DOTween.To(() => moveDownCoef, x => moveDownCoef = x, topSpeedCoef, totalTime/2).OnComplete(() => CoroutineStarter.Instance.StartCoroutine(StopRoll()));
    }

    private void SetMoveState(float delay)
    {
        resultDelay = delay;
        moveDownCoef = rollInitalPull;
        shouldMove = true;
    }

    private IEnumerator StopRoll()
    {
        yield return new WaitForSeconds(resultDelay * 0.2f);

        int IndexDistance = CalculateIndexDistance();

        float totalDistance = itemHeight * IndexDistance;
        MoveAllitems(totalDistance);

        while (true)
        {
            if (targetItemIndex >= 0)
            {
                float distance = visibleItems[targetItemIndex].GetPosition();
                AdjustMoveDownCoef(distance);
                if (Mathf.Approximately(distance, 0))
                    break;
            }
            yield return new WaitForFixedUpdate();
        }
        ResetValues();
    }

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

    

    // returns true if its zero
    private bool AdjustMoveDownCoef(float distance)
    {
        if (distance < 0)
        {
            return false;
        }

        if (distance < 100)
        {
            moveDownCoef = 0f;
            float newPosition = visibleItems[targetItemIndex].GetPosition();
            MoveAllitems(newPosition);
            //MovementStopped?.Invoke();
            return true;
        }

        if (distance > 100 && !fixedOutput)
        {
            fixedOutput = true;
            float newPosition = visibleItems[targetItemIndex].GetPosition();
            MoveAllitems(newPosition - 500);
        }

        return false;
    }

    private void MoveAllitems(float distance)
    {
        foreach(var item in visibleItems)
        {
            item.MoveDown(distance);
        }
    }

    private void ResetValues()
    {
        fixedOutput = false;
        shouldMove = false;
        targetItemIndex = -1;
    }

    private void ReuseItem(List<int> indexes)
    {
        foreach(var index in indexes)
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

    public void Clear()
    {
        itemTypes.Clear();
        itemTypes = null;
    }
}
