using System;

public class TwoSum
{
    public int[] TwoSum1(int[] nums, int target)
    {
        int cumilativeValue = target;
        int lenn = nums.Length;
        Dictionary<int,int> numsMap = new Dictionary<int, int>();
        for(int i = 0; i < lenn; i++)
        {
            cumilativeValue = target-nums[i];
            if (numsMap.ContainsKey(cumilativeValue))
            {
                return new int[]{numsMap[cumilativeValue],i};
            }
            else

                numsMap.TryAdd(nums[i],i);
            }

            return new int[0];
        
        }
    }