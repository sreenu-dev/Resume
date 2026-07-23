using System;

public class ProductArray
{
    public int[] ProductExceptSelf(int[] nums) {
        int totalprod = 1;
        bool hasZero = false;
        bool allZeros = true;
        foreach(int i in nums){
            if (i != 0)
            {
                allZeros = false;
                totalprod *=i;
            }
            else
                hasZero = true;
        }
        Console.WriteLine(totalprod);
        int[] outi = new int[nums.Length];
        for(var i=0;i<outi.Length;i++){
            try
            {
                 outi[i] = hasZero && nums[i]!=0?0:totalprod/nums[i];
            }
            catch (Exception ex)
            {
                outi[i]= allZeros? 0 : totalprod;
            }
        }
        foreach(var i in outi){
            Console.Write(i+",");
        }
        Console.WriteLine();
        return outi;
    }

    public int[] ProductExceptSelf2(int[] nums)
    {
        int n = nums.Length;
        int[] res = new int[n];
        
        // Step 1: Calculate Prefix (Left) products.
        // Store the cumulative product of elements to the left of index i.
        int prefix = 1;
        for (int i = 0; i < n; i++) 
        {
            res[i] = prefix;
            prefix *= nums[i];
        }
        
        // Step 2: Calculate Suffix (Right) products on the fly.
        // Multiply the existing prefix values by the cumulative product to the right.
        int suffix = 1;
        for (int i = n - 1; i >= 0; i--) 
        {
            res[i] *= suffix;
            suffix *= nums[i];
        }
        
        return res;
    }
}