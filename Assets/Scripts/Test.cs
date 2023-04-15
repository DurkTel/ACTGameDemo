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
        //print(LongestPalindrome("babad"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string LongestPalindrome(string s)
    {
        int index = 0;
        int max = 1;
        int length = s.Length;
        for (int i = 0; i < length; i++)
        {
            int len = 1;
            int left = i;
            int right = i;
            while (left >= 0 && right <= length - 1 && s[left] == s[right])
            {
                if (len % 2 == 0)
                {
                    if (len % 2 == 0 && left > 0 && s[i] == s[left - 1])
                    {
                        len++;
                        left--;
                        if (len > max)
                        {
                            max = len;
                            index = left;
                        }
                    }
                    else if (right < length - 1 && s[i] == s[right + 1])
                    {
                        len++;
                        right++;
                        if (len > max)
                        {
                            max = len;
                            index = left;
                        }
                    }
                    
                }
                else if (left > 0 && right < length - 1 && s[left - 1] == s[right + 1])
                {
                    len = len + 2;
                    left--;
                    right++;
                    if (len > max)
                    {
                        max = len;
                        index = left;
                    }
                }
                else
                    break;
            }
        }

        return s.Substring(index, max);
    }

    public int LengthOfLongestSubstring(string s)
    {
        List<char> mark = new List<char>();
        int max = 0;
        int counter = 0;
        for (int i = 0; i < s.Length; i++)
        {
            while (mark.Contains(s[i]))
            {
                counter--;
                mark.RemoveAt(0);
            }
            mark.Add(s[i]);
            counter++;
            max = counter > max ? counter : max;
        }

        return max;
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

    public ListNode AddTwoNumbersEx(ListNode l1, ListNode l2)
    {
        bool flag = false;
        ListNode GetListNode(ListNode l1, ListNode l2)
        {
            if (l1 == null && l2 == null) return null;

            int sum = (l1 == null ? 0 : l1.val) + (l2 == null ? 0 : l2.val);
            int newRe = sum % 10;
            if (flag)
            {
                flag = false;
                newRe++;
            }

            if (l1 != null)
                l1 = l1.next;
            if (l2 != null)
                l2 = l2.next;
            if (sum >= 10)
                flag = true;

            return new ListNode(newRe, GetListNode(l1, l2));
        }

        return GetListNode(l1, l2);
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

