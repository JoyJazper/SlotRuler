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
        targetItemIndex = Random.Range(0, visibleItems.Length);
        targetItemType = visibleItems[targetItemIndex].SlotType;
        CoroutineStarter.Instance.DED(" Random Target : " + targetItemType.ToString());
    }

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
        int reuseItemIndex = -1;
        for (int i = 0; i < visibleItems.Length; i++)
        {
            if (visibleItems[i].GetPosition() <= checkEndHeight)
            {
                reuseItemIndex = i;
            }
            else
            {
                visibleItems[i].MoveDown(rollSpeed);
            }
        }
        if(reuseItemIndex != -1)
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
        //CoroutineStarter.Instance.DED("Time to roll");
        if (targettype != SlotItemType.none)
        {
            targetItemType = targettype;
            CoroutineStarter.Instance.DED(" Target Item from start : " + targetItemType.ToString());
        }
        else
        {
            GenerateTarget();
        }
        DOTween.To(() => moveDownCoef, x => moveDownCoef = x, topSpeedCoef, totalTime/3).OnComplete(() => CoroutineStarter.Instance.StartCoroutine(StopRoll()));
    }

    

    private IEnumerator StopRoll()
    {
        yield return new WaitForSeconds(resultDelay * totalTime / 3);
        float distance = 1000f;
        while (moveDownCoef != 0)
        {
            if(targetItemIndex >= 0)
            {
                distance = visibleItems[targetItemIndex].GetPosition();
            }
            else
            {
                distance = 1000f;
            }
            if (AdjustMoveDownCoef(distance))
            {
                MoveAllitems(distance);
            }

            yield return null;
        }
        
        //slotPanel.DOShakeAnchorPos(0.2f, 20f).OnComplete(() => ResetValues());
        ResetValues();
    }

    float deceleration = 9f;

    // returns true if its zero
    private bool AdjustMoveDownCoef(float distance)
    {

        if (moveDownCoef > 0.8f)
        {
            moveDownCoef -= moveDownCoef / deceleration;
            moveDownCoef = Mathf.Clamp(moveDownCoef, 0.8f, topSpeedCoef);
        }
        if (distance > 0 && distance < 50f)
        {
            moveDownCoef = 0f;
            return true;
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


    

    private void ReuseItem(int index)
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
        else if(lastIndex < 0)
        {
            lastIndex = 0;
        }

        lastTopIndex = index;

        SlotItemType newType = itemTypes[lastIndex];
        
        if(newType == targetItemType)
        {
            CoroutineStarter.Instance.DED(" Target Item from reset : " + targetItemType.ToString());
            targetItemIndex = index;
        }
        visibleItems[index].SetCharacter(newType, info.GetValue(newType));
    }

    public void Clear()
    {
        itemTypes.Clear();
        itemTypes = null;
    }
}
