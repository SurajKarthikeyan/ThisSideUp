using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Generic Class
 * Heap Class used to better handle node searching for optimization
 */
public class Heap<T> where T : IHeapItem<T>
{
 [SerializeField]
 private T[] items;     // Heap
 [SerializeField]
 private int currentItemCount;      // Size
 [SerializeField] private int maxHeapSize;  // Max Size

 public Heap(int maxHeapSize)
 {
     items = new T[maxHeapSize];
     
 }


 
/**
 * Add item to heap
 * param: T item
 */
 public void AddToHeap(T item)
 {
     item.HeapIndex = currentItemCount;     // Set the current item's index to current count
     items[currentItemCount] = item;        // Add it to the heap
     SortToParent(item);        // Sort it based on parent values
     currentItemCount++;
}

/*
 * Pop first item in heap
 * Returns the previously first item
 * Restructures the heap accordingly
 */
 public T RemoveFirst()
 {
     T firstItem = items[0];
     currentItemCount--;
     items[0] = items[currentItemCount];
     items[0].HeapIndex = 0;
     SortToChild(items[0]);
     return firstItem;
 }

 public void UpdateItem(T item)
 {
     SortToParent(item);
     
 }

 /*
  * Getter for currentItemCount
  * Gets the number of items in the heap
  */
 public int Count
 {
     get
     {
         return currentItemCount;
     }
 }

 /*
  * Function to return if the heap contains a specific item
  */
 public bool Contains(T item)
 {
     return Equals(items[item.HeapIndex], item);
 }

 
 /**
  * After removing an item, it resorts the heap
  * Find the child with the highest priority
  * Sets the index to be swapped to that child
  */
 void SortToChild(T item)
 {
     while (true)
     {
         int childIndexLeft = item.HeapIndex * 2 + 1;
         int childIndexRight = item.HeapIndex * 2 + 2;
         int swapIndex = 0;
         
         if (childIndexLeft < currentItemCount) //check priority of left child
         {
             swapIndex = childIndexLeft;
             if (childIndexRight < currentItemCount)    //check against right
             {
                 if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                 {
                     swapIndex = childIndexRight;
                 }
             }

             if (item.CompareTo(items[swapIndex]) < 0)  // check against parent
             {
                 Swap(item, items[swapIndex]);
             }
             else
             {
                 return;
             }
         }
         else
         {
             return;
         }
         

     }
 }
 
 /**
  * Function to sort the heap based on Fcost
  */
 public void SortToParent(T item)
 {
     int parentIndex = (item.HeapIndex - 1) / 2;        //Formula for parent index
     while (true)
     {
         T parentItem = items[parentIndex];
         if (item.CompareTo(parentItem) > 0)        // if it has a higher priority than parent item(lower FCost)
         {
             Swap(item, parentItem);        //Sort
         }
         else
         {
             break;                                     // if its not, just break
         }
         parentIndex = (item.HeapIndex - 1) / 2;        //recalculate parent index

     }
 }

 
 /**
  * Swaps the values of the items and the indices they are at in the function
  */
 public void Swap(T itemAlpha, T itemBeta)
 {
     items[itemAlpha.HeapIndex] = itemBeta;
     items[itemBeta.HeapIndex] = itemAlpha;

     int itemAlphaTempIndex = itemAlpha.HeapIndex;
     itemAlpha.HeapIndex = itemBeta.HeapIndex;
     itemBeta.HeapIndex = itemAlphaTempIndex;
 }
}


//Interface used to create generic item description
//Allows for functionality like keeping track of index
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex // Allows for HeapItem to keep track of its own index
    {
        get;
        set;
    }
}
