using System;
using System.Collections.Generic;
using PriorityQueue;

namespace Observer {

	// P is passed onto the Subject
	// A is passed onto the Observer(s) from the Subject
	public abstract class Subject<O, TInput, TOutput> where O : IListener<TOutput> {

		protected Dictionary<int, O> Observers;	// container of all observers according to their ID number
		private PriorityQueue<int> _pendingIDNumbers;	// ID numbers of previously removed elements
		private int _nextHighestID;

		public Subject(){

			Observers = new Dictionary<int, O>();
			_pendingIDNumbers = new PriorityQueue<int>(true);
			_nextHighestID = 0;

		}

		// add observer to container of observers
		public void Attach(O observer){

			// if available, use the ID number of a previously removed element
			int index = _pendingIDNumbers.Count > 0 ? _pendingIDNumbers.Dequeue() : _nextHighestID++;
			Observers[index] = observer;
			observer.SetID(index);

		}

		// remove observer from container of observers
		public void Detach(O observer){

			// get the ID of the given observer
			int index = observer.GetID();

			// if the observer is not contained, do not proceed
			if(!Observers.ContainsKey(index))
				return;

			// remove the observer at that index and enqueue its ID to be reused by a new observer
			Observers.Remove(index);
			_pendingIDNumbers.Enqueue(index);

		}

		// notify all observers of arguments
		public abstract void Notify(TInput argument);

	}

	public interface IListener<N> {

		void SetID(int index);	// set ID of this element
		int GetID();	// get ID of this element
		void Update(N argument);	// act according to given arguments

	}

	public abstract class BasicObserver<N> : IListener<N> {

		private int _ID;

		public void SetID(int index){ _ID = index; }
		public int GetID() { return _ID; }
		public abstract void Update(N argument);

	}

}