using System.Collections.Generic;
using System;
using Queue;

namespace PriorityQueue {
	
	public class PriorityQueue<T> : IQueue<T> where T : IComparable<T> {

		private List<T> _elements;
		private int _count;
		private bool _reverse;

		public int Count { get { return _count; } }

		// by default, the comparer is not used in reverse
		public PriorityQueue() : this(false) { }

		public PriorityQueue(bool reverse){

        	_elements = new List<T>();
			_reverse = reverse;
			_count = 0;

		}

		public void Add(T element){

			// if contents are empty, add element to the front so that the index is taken
			if(_count == 0)
				_elements.Add(element);

			// add element to end of list
			_elements.Add(element);
			_count++;	//index of currently added item

			// upheap latest element
			Upheap(_count);

		}

		public T Poll(){

			if(_count == 0)
				return default(T);

			T head = _elements[1];	//get the topmost element
			
			// if there is just 1 element in the queue, return the head and remove it from the queue
			if(_count == 1){
				_elements.RemoveAt(1);
				_elements.RemoveAt(0);	// also remove the stand-in element
				_count--;	// there's one less element in the queue
				return head;
			}

			// otherwise, swap with the bottommost element and downheap from the top
			T last = _elements[_count];
			_elements.RemoveAt(_count);
			_count--;	// there's one less element in the queue
			_elements[1] = last;
			Downheap(1);
			return head;

		}

		public void Remove(T element){



		}

		public T Peek() {

			if(_count == 0)
				return default(T);
			
			return _elements[1];

		}

		public override string ToString(){

			string s = " (";

			// if there's something at the top of this queue, print that first
			if(_count >= 1)
				s += _elements[1];

			// print elements following the first, if any
			for(int i = 2; i <= _count; i++)
				s += ", " + _elements[i];

			return s + ")";

		}

		private int GetParentIndex(int childIndex) { return childIndex / 2; }
		private int GetLeftChildIndex(int parentIndex) { return parentIndex * 2; }	// left children are always LEFT
		private int GetRightChildIndex(int parentIndex) { return parentIndex * 2 + 1; }	// right children are always ODD
		private int CompareElements(T x, T y) {

			if(x == null || y == null)
				return 0;

			return x.CompareTo(y) * (_reverse ? -1 : 1);

		}

		private void Upheap(int childIndex){

			// do not proceed if the childIndex == 1, otherwise its parent will be 0
			if(childIndex == 1)
				return;

			// get parentIndex
			int parentIndex = GetParentIndex(childIndex);

			// get elements
			T child = _elements[childIndex];
			T parent = _elements[parentIndex];

			// if the parent is smaller than the child element
			if(CompareElements(parent, child) < 0){

				// swap elements
				_elements[parentIndex] = child;
				_elements[childIndex] = parent;

				// call sideheap to check if the children nodes of the new parent shall be switched
				Sideheap(childIndex % 2 == 1 ? childIndex - 1 : childIndex);

				// now upheap at the parent's position
				Upheap(parentIndex);

			}
			
			Sideheap(childIndex % 2 == 1 ? childIndex - 1 : childIndex);

		}

		private void Sideheap(int leftIndex){

			int rightIndex = leftIndex + 1;	// rightIndex is always leftIndex + 1

			// if the rightIndex does not exist, do not proceed
			if(rightIndex > _count)
				return;

			// get elements
			T left = _elements[leftIndex];
			T right = _elements[rightIndex];

			// if left is less than right, swap
			if(CompareElements(left, right) < 0){

				//Console.WriteLine("Swapped " + right + " and " + left);
				_elements[leftIndex] = right;
				_elements[rightIndex] = left;

			}

		}

		private void Downheap(int parentIndex){

			// get indices of children
			int leftIndex = GetLeftChildIndex(parentIndex);
			int rightIndex = GetRightChildIndex(parentIndex);

			// check if these are out of bounds
			bool leftOOB = leftIndex > _count;
			bool rightOOB = rightIndex > _count;

			// if both are out of bounds, there's nothing to compare to
			if(leftOOB && rightOOB)
				return;

			// get elements
			T parent = _elements[parentIndex];
			T left = default(T);
			if(!leftOOB)
				left = _elements[leftIndex];
			T right = default(T);
			if(!rightOOB)
				right = _elements[rightIndex];

			// if parent is less than either of its children
			if(CompareElements(parent, left) < 0 || CompareElements(parent, right) < 0){

				// if left does not exist, this should NOT happen
				if(leftOOB){
					Console.WriteLine("Left element SHOULD EXIST");
					return;
				}

				// swap parent and left child
				//Console.WriteLine("Swapped " + parent + " and " + left);
				_elements[parentIndex] = left;
				_elements[leftIndex] = parent;
				Downheap(leftIndex);

			}
			
			Sideheap(leftIndex);

		}

	}

}