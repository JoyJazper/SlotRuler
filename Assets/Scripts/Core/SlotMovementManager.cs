using System.Collections;
using UnityEngine;
using DG.Tweening;
using JoyKit.Utility;
using System.Collections.Generic;
using System;
public class SlotMovementManager
{
    public event Action MovementStopped;
    private readonly float moveDownSpeed = 100f;
    private readonly float topSpeedCoef = 1f;
    private readonly float totalTime = 1.5f;
    private readonly float rollInitalPull = -0.5f;

    private RectTransform slotPanel;
    private SlotItemInfo info;

    private float moveDownCoef = 1f, checkEndHeight = 0f, itemHeight = 200f, resultDelay = 0f;
    public bool shouldMove = false;

    private int lastTopIndex = -1, targetItemIndex = -1;
    private SlotItemType targetItemType = SlotItemType.none;
    private SlotItem[] visibleItems;
    private List<SlotItemType> itemTypes = new List<SlotItemType>();

    public SlotMovementManager(RectTransform slotPanel, int topItemIndex, ref SlotItemInfo info, ref SlotItem[] visibleItems)
    {
        this.slotPanel = slotPanel;
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
        resultDelay = delay;
        moveDownCoef = rollInitalPull;
        shouldMove = true;
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

    

    private IEnumerator StopRoll()
    {
        yield return new WaitForSeconds(resultDelay * 0.2f);
        float distance = 1000f;
        fixedOutput = false;
        int skipCount;
        int index1 = itemTypes.IndexOf(visibleItems[lastTopIndex].SlotType);
        int index2 = itemTypes.IndexOf(targetItemType);
        if (index1 < index2)
        {
            skipCount = index2 - index1;
        }
        else
        {
            skipCount = itemTypes.Count - (index1 - index2);
        }
        MoveAllitems(itemHeight * skipCount);

        do
        {
            if (targetItemIndex >= 0)
            {
                distance = visibleItems[targetItemIndex].GetPosition();
                AdjustMoveDownCoef(distance);
            }
            yield return new WaitForFixedUpdate();
        } while (distance != 0);
        ResetValues();
    }

    bool fixedOutput  = false;

    // returns true if its zero
    private bool AdjustMoveDownCoef(float distance)
    {
        if (distance >= 0)
        {
            if(distance < 100)
            {
                moveDownCoef = 0f;
                distance = visibleItems[targetItemIndex].GetPosition();
                MoveAllitems(distance);
                MovementStopped?.Invoke();
                return true;
            }
            else if(distance > 100 && !fixedOutput)
            {
                fixedOutput = true;

                distance = visibleItems[targetItemIndex].GetPosition();
                MoveAllitems(distance - 500);
            }
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
                /*CoroutineStarter.Instance.DED(" Target Item from reset : " + targetItemType.ToString());*/
                targetItemIndex = index;
            }
            visibleItems[index].SetCharacter(newType, info.GetValue(newType));
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
