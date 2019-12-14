using System.Collections.Generic;
using System;
using Queue;

namespace PriorityQueue {
	
	public class PriorityQueue<T> : IQueue<T> where T : IComparable<T> {

		private List<T> _elements;
		private int _count;
		private bool _reverse;

		private const int DefaultCapacity = 100;

		public int Count { get { return _count; } }
		public bool IsEmpty { get { return _count == 0; } }

		// by default, the comparer is not used in reverse
		public PriorityQueue() : this(DefaultCapacity, false) { }
		public PriorityQueue(int capacity) : this(capacity, false) { }
		public PriorityQueue(bool reverse) : this(DefaultCapacity, reverse) { }

		public PriorityQueue(int capacity, bool reverse){

        	_elements = new List<T>(capacity);
			_reverse = reverse;
			_count = 0;

		}

		public void Enqueue(T element){

			// if contents are empty, add element to the front so that the index is taken
			if(_count == 0)
				_elements.Add(element);

			// add element to end of list
			_elements.Add(element);
			_count++;	//index of currently added item

			// upheap latest element
			Upheap(_count);

		}

		public T Dequeue(){

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

		public void Update(T element){

			// remove element if it exists
			Remove(element);

			//add it back to the queue
			Enqueue(element);

		}

		// private function to assist Remove(T element) function
		private T RemoveAt(int index){

			T element = _elements[index];	//get the topmost element
			
			// if there is just 1 element in the queue, return the head and remove it from the queue
			if(_count == 1){
				_elements.RemoveAt(1);
				_elements.RemoveAt(0);	// also remove the stand-in element
				_count--;	// there's one less element in the queue
				return element;
			}

			// otherwise, swap with the bottommost element and downheap from the top
			string status = this.ToString();
			T last = _elements[_count];
			_elements.RemoveAt(_count);
			_count--;	// there's one less element in the queue

			// if it wasn't the last element we were removing, move the last element to index and downheap
			if(index <= _count){ 
				_elements[index] = last;
				Downheap(index);
			}

			return element;
			

		}

		public T Peek() {

			if(_count == 0)
				return default(T);
			
			return _elements[1];

		}

		public override string ToString(){

			string s = " {";

			// if there's something at the top of this queue, print that first
			if(_count >= 1)
				s += _elements[1];

			// print elements following the first, if any
			for(int i = 2; i <= _count; i++)
				s += ", " + _elements[i];

			return s + "}";

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

				// find largest child
				int largestIndex = -1;
				if(!leftOOB && !rightOOB)	// if there are elements to the left and right, choose the largest of the two
					largestIndex = CompareElements(left, right) > 0 ? leftIndex : rightIndex;
				else if(!leftOOB && CompareElements(parent, left) < 0)	// if there is only an element to the left, make sure also that it is larger priority than the parent
					largestIndex = leftIndex;

				// this is because default(int) is 0, so it will not be considered null and is still being compared
				//		despite not existing, so we need to make sure that there actually is a 0 there
				//		and we're not just comparing the parent to a non-existent element

				// if not found, don't proceed
				if(largestIndex == -1)
					return;
				
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

	public class PriorityQueue<TPriority, TDatum> where TPriority : IComparable<TPriority>{
		
		private const int DefaultCapacity = 100;

		private PriorityQueue<PriorityNode<TPriority, TDatum>> _priorityQueue;

		public int Count { get { return _priorityQueue.Count; } }
		public bool IsEmpty { get { return _priorityQueue.IsEmpty; } }

		public PriorityQueue() : this(DefaultCapacity, false){ }
		public PriorityQueue(int capacity) : this(capacity, false){ }
		public PriorityQueue(bool reverse) : this(DefaultCapacity, reverse){ }
		public PriorityQueue(int capacity, bool reverse){ _priorityQueue = new PriorityQueue<PriorityNode<TPriority, TDatum>>(capacity, reverse); }

		public void Enqueue(TPriority priority, TDatum datum){

			PriorityNode<TPriority, TDatum> entry = new PriorityNode<TPriority, TDatum>(priority, datum);
			_priorityQueue.Enqueue(entry);

		}

		public TDatum Peek(){

			return _priorityQueue.Peek() != null ? _priorityQueue.Peek().Datum : default(TDatum);

		}

		public TDatum Dequeue(){

			return _priorityQueue.Peek() != null ? _priorityQueue.Dequeue().Datum : default(TDatum);

		}

		public void Remove(TDatum datum){

			PriorityNode<TPriority, TDatum> temp = new PriorityNode<TPriority, TDatum>(default(TPriority), datum);
			_priorityQueue.Remove(temp);

		}

		public void Update(TPriority priority, TDatum datum){

			PriorityNode<TPriority, TDatum> temp = new PriorityNode<TPriority, TDatum>(priority, datum);
			_priorityQueue.Update(temp);

		}

	}

	public class PriorityNode<TPriority, TDatum> : IComparable<PriorityNode<TPriority, TDatum>> where TPriority : IComparable<TPriority>{

		public TPriority Priority { get; private set; }
		public TDatum Datum { get; private set; }

		public PriorityNode(TPriority priority, TDatum datum){

			Priority = priority;
			Datum = datum;

		}

		public int CompareTo(PriorityNode<TPriority, TDatum> other){

			return other.Priority.CompareTo(Priority);

		}

		public override bool Equals(object obj){

			if(obj.GetType() != GetType())
				return false;

			PriorityNode<TPriority, TDatum> other = (PriorityNode<TPriority, TDatum>)obj;

			if(other.Datum.GetType() != Datum.GetType())
				return false;
			
			return other.Datum.Equals(Datum);
		}

		public override int GetHashCode(){

			return Datum.GetHashCode();

		}

	}

}