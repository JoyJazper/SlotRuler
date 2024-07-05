using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Slot : MonoBehaviour
{
    [SerializeField] private RectTransform SlotPanel;
    [SerializeField] private SlotItem template;

    [SerializeField] private SlotItemInfo slotInfo;

    private SlotItem[] visibleItems = new SlotItem[7];

    private SlotItem lastTop = null;
    private SlotItem newLastTop = null;

    [SerializeField] private float moveDownSpeed = 100f;

    [SerializeField] private SlotItemType target;
    [SerializeField] private SlotItem targetItem;

    private float moveDownCoef = 1f;
    private float rollSpeed, compareValue;
    private short roll = 1;
    private SlotItem initialLastTop;

    [SerializeField] private bool shouldMove = false;

    private void Start()
    {
        SetupPlayGround();
        compareValue = -3f * template.Height;

        initialLastTop = lastTop;
    }

    List<SlotItemType> items;
    private float[] setPositions = new float[7];
    private void SetupPlayGround()
    {

        items =  slotInfo.GetSlotItemList();

        // Place center
        int middle = 3;
        setPositions[middle] = 0.0f;
        visibleItems[middle] = GenerateItem(items[middle], setPositions[middle]);

        // Place above
        setPositions[4] = setPositions[3] + template.Height;
        setPositions[5] = setPositions[3] + 2f * template.Height;
        setPositions[6] = setPositions[3] + 3f * template.Height;

        visibleItems[4] = GenerateItem(items[(middle + 1) % items.Count], setPositions[4]);
        visibleItems[5] = GenerateItem(items[(middle + 2) % items.Count], setPositions[5]);
        visibleItems[6] = GenerateItem(items[(middle + 3) % items.Count], setPositions[6]);
        lastTop = visibleItems[6];
        initialLastTop = lastTop;
        // Place below
        setPositions[0] = setPositions[3] - 3f * template.Height;
        setPositions[1] = setPositions[3] - 2f * template.Height;
        setPositions[2] = setPositions[3] - template.Height;

        visibleItems[0] = GenerateItem(items[(middle - 3 + items.Count) % items.Count], setPositions[0]);
        visibleItems[1] = GenerateItem(items[(middle - 2 + items.Count) % items.Count], setPositions[1]);
        visibleItems[2] = GenerateItem(items[(middle - 1 + items.Count) % items.Count], setPositions[2]);

    }

    private SlotItem GenerateItem(SlotItemType type, float yPosition)
    {
        SlotItem newItem = Instantiate(template.gameObject, SlotPanel).GetComponent<SlotItem>();
        newItem.SetCharacter(type, slotInfo.GetValue(type));
        newItem.SetPosition(yPosition);
        return newItem;
    }

    private void Update()
    {
        if(shouldMove) 
        {
            Move();
        }
    }

    public void OnRollPress()
    {
        StartRoll();
    }

    private void StartRoll()
    {
        // Move a little back
        moveDownCoef = -0.2f;
        shouldMove = true;
        roll = 1;
        if(targetItem == null)
        {
            int rand = UnityEngine.Random.Range(0, visibleItems.Length - 1);
            targetItem = visibleItems[rand];
        }
        // Start the rolling process after moving back
        DOTween.To(() => moveDownCoef, x => moveDownCoef = x, 1f, 2f).OnComplete(() => CoverRoll());
    }

    private void CoverRoll()
    {
        StopAllCoroutines();
        Stop();  
    }

    private void Stop()
    {
        DOTween.To(() => moveDownCoef, x => moveDownCoef = x, 1.5f, 1f).OnComplete(() => StartCoroutine(SlowDownRoll()));
        //DOTween.To(() => moveDownCoef, x => moveDownCoef = x, 1.5f, 1f).OnComplete(() =>
        //    DOTween.To(() => moveDownCoef, x => moveDownCoef = x, 0.5f, 1f).OnComplete(() => StartCoroutine(SlowDownRoll()))
        //);
    }

    IEnumerator SlowDownRoll()
    {
        //Debug.LogError("ERNOS : Starting the end.");
        float distance = targetItem.GetPosition();
        while(distance != 0)
        {
            distance = targetItem.GetPosition();
            if(moveDownCoef > 0.2f)
            {
                if (distance < 500f && distance > 80f)
                {
                    moveDownCoef = moveDownCoef - moveDownCoef / MathF.Abs(distance);
                    //Debug.LogError("ERNOS : Phase 1 : " + distance.ToString() + " and speed coef = " + moveDownCoef);
                }
            }
            else
            {
                if (distance < 2f && distance > -2f)
                {
                    //Debug.LogError("ERNOS : Phase 3 : " + distance.ToString() + " and speed coef = " + moveDownCoef);
                    moveDownCoef = 0f;
                    shouldMove = false;
                    for (int i = 0; i < visibleItems.Length; i++)
                    {
                        {
                            visibleItems[i].MoveDown(distance);
                        }
                    }
                }
            }
            yield return null;
        }
        
        SlotPanel.DOShakeAnchorPos(0.2f, 20f);
        //Debug.LogError("ERNOS : result : " + targetItem.SlotType.ToString());
        shouldMove = false;
        targetItem = null;
        yield return null;

    }
    
    private void CountCompletedSpin()
    {
        if (lastTop == initialLastTop)
            roll++;
    }

    private void Move()
    {
        rollSpeed = moveDownSpeed * (100f * Time.deltaTime) * moveDownCoef;
        for (int i = 0; i < visibleItems.Length; i++)
        {
            if (visibleItems[i].GetPosition() <= compareValue)
            {
                newLastTop = visibleItems[i];
            }
            else
            {
                visibleItems[i].MoveDown(rollSpeed);
            }
        }
        if (newLastTop != null)
            Reuse(newLastTop);
    }

    private void Reuse(SlotItem item)
    {
        // Set the position of the item
        float newYPosition = lastTop.GetPosition() + lastTop.Height;
        item.SetPosition(newYPosition);

        // Get the index of the next slot type
        int lastIndex = items.IndexOf(lastTop.SlotType) + 1;
        if (lastIndex >= items.Count)
        {
            lastIndex = 0;
        }

        // Set the new slot type and character
        SlotItemType newType = items[lastIndex];
        item.SetCharacter(newType, slotInfo.GetValue(newType));

        // Update the lastTop reference
        lastTop = item;
        CountCompletedSpin();
        newLastTop = null;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
