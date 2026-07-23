public class MoveZeroes()
{
    public void MoveZeroes1(int[] nums)
    {
        int lenn = nums.Length;
        int zeroesIndex = 0;
        for(int i = 0; i < lenn; i++)
        {
            if (nums[i] != 0)
            {
                nums[zeroesIndex] = nums[i];
                zeroesIndex++;
            }
        }

        while (zeroesIndex < lenn)
        {
            nums[zeroesIndex] =0;
            zeroesIndex++;
        }

        foreach(var i in nums)
        {
            Console.Write(i+",");
        }
        Console.WriteLine();
    }
}