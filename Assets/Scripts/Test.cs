using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class Test : MonoBehaviour
{
    public struct sdas
    {
        public int ss;
    }

    void Start()
    {
        ListNode re = AddTwoNumbers(new ListNode(9), new ListNode(1, new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9)))))))))));
        while (true)
        {
            if (re == null) break;
            print(re.val);
            re = re.next;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int[] TwoSum(int[] nums, int target)
    {
        Dictionary<int, int> result = new Dictionary<int, int>();   
        for (int i = 0; i < nums.Length; i++)
        {
            int delta = target - nums[i];
            if (result.ContainsKey(delta))
                return new int[] { i, result[delta] };
            else
                result[nums[i]] = i;
        }
        return null;
    }

    public class ListNode
    {
        public int val;
        public ListNode next;
        public ListNode(int val = 0, ListNode next = null)
        {
            this.val = val;
            this.next = next;
        }
    }
    public ListNode AddTwoNumbers(ListNode l1, ListNode l2)
    {
        int GetNum(ListNode l)
        {
            float i = 0;
            int flag = 1;
            while (true)
            {
                i += l.val;
                l = l.next;
                if (l != null)
                {
                    i /= 10f;
                    flag *= 10;
                }
                else
                    break;
            }
            return (int)(i * flag + 0.5f);
        }

        int num1 = GetNum(l1);
        int num2 = GetNum(l2);
        int sum = num1 + num2;
        print(sum);
        if (sum == 0) return new ListNode(0);
        ListNode re;
        ListNode GetListNode(int sum)
        {
            if (sum == 0) return null;
            float ori = sum;
            sum = (int)(sum / 10f);
            int afterPoint = (int)(ori - sum * 10);
            re = new ListNode(afterPoint, GetListNode(sum));
            return re;
        }

        return GetListNode(sum);
    }
}

