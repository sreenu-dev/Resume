public class ContinerWithMostWater
{
    public int maxArea(int[] height)
    {
        int maxArea =0;
        int left = 0;
        int right = height.Length-1;
        while (left < right)
        {
            int area = (right-left)*Math.Min(height[left],height[right]);
            maxArea = Math.Max(maxArea,area);
            if (height[left] < height[right])
            {
                left++;
            }
            else
            {
                right--;
            }
        }
        return maxArea;
    }
}