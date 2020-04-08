using System;

/// <summary>
/// Data structure resembling a tree, used to get the highest priority item with performance.
/// </summary>
/// <typeparam name="T">Generic type for the heap item.</typeparam>
public class BinaryHeap<T> where T : IComparable, IHeapable
{
	private T[] items;
	private int itemCount;

	public T[] GetArray()
	{
		/* Might cause some issues because probably the tree is not full 
		and some index might be empty.*/
		return items;
	}

	public int Count
	{
		get
		{
			return itemCount;
		}
	}

	/// <summary>
	/// Creates a new BinaryHeap.
	/// </summary>
	/// <param name="size">Max ammount of items.</param>
	public BinaryHeap(int size)
	{
		items = new T[size];
	}

	/// <summary>
	/// Sorts the item down the tree.
	/// </summary>
	/// <param name="item">Item to be sorted.</param>
	public void SortDown(T item)
	{
		// If the item is swapped, it must pass trough the loop again.
		while (true)
		{
			/* Using formula (index * 2) + 1 and (index * 2) + 2
			 to get the index from the left and right child, respectively.*/
			int leftIndex = item.Index * 2 + 1;
			int rightIndex = item.Index * 2 + 2;

			// Checking if the children exist.
			bool hasLeftChild = leftIndex < itemCount;
			bool hasRightChild = rightIndex < itemCount;

			/* Checking if the children have more priority than the item 
			 and then swapping if they do (right child preferred).*/
			if (hasLeftChild)
			{
				T leftChild = items[leftIndex];
				if (hasRightChild)
				{
					T rightChild = items[rightIndex];
					if (rightChild.CompareTo(leftChild) > 0)
					{
						if (rightChild.CompareTo(item) > 0)
						{
							Swap(rightChild, item);
							continue;
						}
					}
				}
				if (leftChild.CompareTo(item) > 0)
				{
					Swap(leftChild, item);
					continue;
				}
			}
			break;
		}
	}

	/// <summary>
	/// Sorts the item up the tree.
	/// </summary>
	/// <param name="item">Item to be sorted.</param>
	public void SortUp(T item)
	{
		// If the item is swapped, it must pass trough the loop again.
		while (true)
		{
			// The first item doesn't have parents, so it can't be sorted.
			if (item.Index == 0)
				return;

			// Using formula (index - 1) / 2 to get the parent of the item.
			int parentIndex = (item.Index - 1) / 2;
			T parent = items[parentIndex];

			// If the parent has higher priority than the item, they are swapped.
			if (item.CompareTo(parent) > 0)
			{
				Swap(item, parent);
				continue;
			}
			break;
		}
	}

	/// <summary>
	/// Swaps itemA and itemB
	/// </summary>
	private void Swap(T itemA, T itemB)
	{
		items[itemA.Index] = itemB;
		items[itemB.Index] = itemA;
		int indexA = itemA.Index;
		itemA.Index = itemB.Index;
		itemB.Index = indexA;
	}

	/// <summary>
	/// Adds an item to the tree.
	/// </summary>
	/// <param name="item">Item to be added.</param>
	public void Add(T item)
	{
		items[itemCount] = item;
		item.Index = itemCount;
		SortUp(item);
		itemCount++;
	}

	/// <summary>
	/// Removes an item from the tree.
	/// </summary>
	/// <param name="item">Item to be removed.</param>
	public void Remove(T item)
	{
		itemCount--;
		T lastItem = items[itemCount];
		Swap(item, lastItem);
		SortDown(lastItem);
		/* Detail: the item is not really removed, it will 
		only be replaced the next time another item is added. */
	}

	/// <summary>
	/// Removes the first item (the one with the highest priority) from the tree.
	/// </summary>
	/// <returns>Item removed.</returns>
	public T RemoveFirst()
	{
		T firstItem = items[0];
		Remove(firstItem);
		return firstItem;
	}

	/// <summary>
	/// Sorts and item up and down.
	/// </summary>
	/// <param name="item"></param>
	public void Update(T item)
	{
		SortUp(item);
		SortDown(item);
	}

	/// <summary>
	/// Checks if the item exists in the tree.
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public bool Contains(T item)
	{
		/* This block is necessary because the items could keep their old index even 
		after the (former) binary heap is destroyed.*/
		if (items[item.Index] == null)
			return false;

		bool contains = items[item.Index].Equals(item);
		return contains;
	}

	public override string ToString()
	{
		string msg = "{";
		if (itemCount == 0)
		{
			return msg + "null}";
		}
		for (int i = 0; i < itemCount; i++)
		{
			msg += items[i] + ", ";
		}
		msg = msg.Remove(msg.Length - 2, 2) + '}';
		return msg;
	}
}
