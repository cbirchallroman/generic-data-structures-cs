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
			
			RemoveAt(1);

			return head;

		}

		public void Remove(T element){

			// find index of the element to be removed
			int index = FindIndex(element);

			// if element not found, don't proceed
			if(index == -1)
				return;

			// remove the element at the index
			RemoveAt(index);

		}

		// private function to assist Remove(T element) function
		private void RemoveAt(int index){

			T element = _elements[index];	//get the topmost element
			
			// if there is just 1 element in the queue, return the head and remove it from the queue
			if(_count == 1){
				_elements.RemoveAt(1);
				_elements.RemoveAt(0);	// also remove the stand-in element
				_count--;	// there's one less element in the queue
				return;
			}

			// otherwise, swap with the bottommost element and downheap from the top
			T last = _elements[_count];
			_elements.RemoveAt(_count);
			_count--;	// there's one less element in the queue
			_elements[index] = last;
			Downheap(index);

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

				// now upheap at the parent's position
				Upheap(parentIndex);

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

				//find largest child
				int largestIndex = CompareElements(left, right) > 0 ? leftIndex : rightIndex;
				T select = _elements[largestIndex];

				// swap parent and largest child
				_elements[parentIndex] = select;
				_elements[largestIndex] = parent;
				Downheap(leftIndex);

			}

		}

		private int FindIndex(T element){

			for(int i = 1; i <= _count; i++)
				if(_elements[i].Equals(element))
					return i;
			
			return -1;

		}

	}

}