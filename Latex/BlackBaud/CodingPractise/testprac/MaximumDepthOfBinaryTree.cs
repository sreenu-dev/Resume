using System;

public class MaximumDepthOfBinaryTree()
{
    public class TreeNode
    {
        public int val;
        public TreeNode left;
        public TreeNode right;
        public TreeNode(int val=0, TreeNode left=null, TreeNode right = null)
        {
            this.val = val;
            this.left = left;
            this.right = right;
        }
    }

    public int MaxDepth(TreeNode root)
    {
        return TraverseTree(root,0);
    }

    public TreeNode CreateTreeNodes(int[] vals)
    {
        TreeNode root = new TreeNode(vals[0]);
        AddValues(root,1,vals);
        Print(root);
        return root;
        
    }

    public void AddValues(TreeNode node,int i,int[] vals)
    {
        if (i < vals.Length)
        {
            node.left = new TreeNode(vals[i]);
            i+=1;
        }
        if (i < vals.Length)
        {
            node.right = new TreeNode(vals[i]);
            i+=1;
        }
        if (i < vals.Length)
        {
            AddValues(node.left,i,vals);
            AddValues(node.right,i,vals);
        }


    }

    public void AddValues2(TreeNode node,TreeNode parent,string direction,int i,int[] vals)
    {
        if (parent != null && node==null)
        {
            node = parent.right;
        }
        if (i < vals.Length)
        {
            if(node.left == null)
            {
                node.left = new TreeNode(vals[i]);
                i+=1;
                AddValues2(node,parent,"right",i,vals);  
            }
            else if(node.right == null)
            {
                node.right = new TreeNode(vals[i]);
                i+=1;
                AddValues2(node,parent,"left",i,vals);
            }
            else
            {
                if (parent == null)
                {
                    AddValues2(null,node,"right",i,vals);
                }
            }
        }
    }
        

    public static void Print(TreeNode root)
    {
        if (root == null)
        {
            Console.WriteLine("<Empty Tree>");
            return;
        }

        int maxDepth = GetDepth(root);
        List<TreeNode> currentLevel = new List<TreeNode> { root };
        int level = 0;

        while (currentLevel.Count > 0 && level < maxDepth)
        {
            // Calculate dynamic spacing based on tree depth
            int floor = maxDepth - level;
            int endLines = (int)Math.Pow(2, Math.Max(floor - 1, 0)) - 1;
            int firstSpaces = (int)Math.Pow(2, floor) - 1;
            int betweenSpaces = (int)Math.Pow(2, floor + 1) - 1;

            PrintSpaces(firstSpaces);

            List<TreeNode> nextLevel = new List<TreeNode>();
            foreach (var node in currentLevel)
            {
                if (node != null)
                {
                    Console.Write(node.val);
                    nextLevel.Add(node.left);
                    nextLevel.Add(node.right);
                }
                else
                {
                    Console.Write(" ");
                    nextLevel.Add(null);
                    nextLevel.Add(null);
                }
                PrintSpaces(betweenSpaces - (node != null ? node.val.ToString().Length - 1 : 0));
            }
            Console.WriteLine();

            // Print the connecting slash lines (/, \)
            if (level < maxDepth - 1)
            {
                for (int i = 1; i <= endLines; i++)
                {
                    for (int j = 0; j < currentLevel.Count; j++)
                    {
                        PrintSpaces(firstSpaces - i);
                        if (currentLevel[j] == null)
                        {
                            PrintSpaces(endLines + endLines + i + 1);
                            continue;
                        }

                        Console.Write(currentLevel[j].left != null ? "/" : " ");
                        PrintSpaces(i + i - 1);
                        Console.Write(currentLevel[j].right != null ? "\\" : " ");
                        PrintSpaces(endLines + endLines - i + 2);
                    }
                    Console.WriteLine();
                }
            }

            // Stop if the next level contains nothing but null pointers
            bool hasMoreNodes = false;
            foreach (var n in nextLevel)
            {
                if (n != null) { hasMoreNodes = true; break; }
            }

            currentLevel = hasMoreNodes ? nextLevel : new List<TreeNode>();
            level++;
        }
    }

    private static int GetDepth(TreeNode node)
    {
        if (node == null) return 0;
        return Math.Max(GetDepth(node.left), GetDepth(node.right)) + 1;
    }

    private static void PrintSpaces(int count)
    {
        for (int i = 0; i < count; i++) Console.Write(" ");
    }
    private static void Print(TreeNode node, string indent, bool isLast, string label)
    {
        if (node == null) return;

        Console.Write(indent);
        Console.Write(isLast ? "└─ " : "├─ ");
        
        // Adds (L) or (R) labels so you instantly know which branch is which
        string branchLabel = string.IsNullOrEmpty(label) ? "" : $"({label}) ";
        Console.WriteLine($"{branchLabel}{node.val}");

        indent += isLast ? "   " : "│  ";

        // Evaluate child branches
        if (node.left != null && node.right != null)
        {
            Print(node.left, indent, false, "L");
            Print(node.right, indent, true, "R");
        }
        else if (node.left != null)
        {
            Print(node.left, indent, true, "L");
        }
        else if (node.right != null)
        {
            Print(node.right, indent, true, "R");
        }
    }

    public int TraverseTree(TreeNode root,int n)
    {
        if(root == null)
        {
            return n;
        }
        else
        {
            return Math.Max(TraverseTree(root.left,n+1),TraverseTree(root.right,n+1));
        }
    }
}