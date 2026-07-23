﻿// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// 1. Create an instance of your new class
var myObject = new MyNewClass();
var twoSumObject = new TwoSum();
var productArray = new ProductArray();
var moveZeroes = new MoveZeroes();

// 2. Call the method from the new class instance
myObject.SaySomething();

Console.WriteLine(twoSumObject.TwoSum1(new int[]{3,3},6));

moveZeroes.MoveZeroes1(new int[]{0,1,0,3,12});

// int[] inp = [1,2,3,4];
// int[] inp2 = [-1,1,0,-3,3];
// Console.WriteLine(productArray.ProductExceptSelf(inp2));
// productArray.ProductExceptSelf2(inp);
