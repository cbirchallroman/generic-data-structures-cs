using System.Collections.Generic;
using PriorityQueue;

namespace Observer {

	public abstract class Subject<T> where T : IObserver<T> {

		private Dictionary<int, T> _observers;	// container of all observers according to their ID number
		private PriorityQueue<int> _pendingIDNumbers;	// ID numbers of previously removed elements
		private int _nextHighestID;

		public Subject(){

			_observers = new Dictionary<int, T>();
			_pendingIDNumbers = new PriorityQueue<int>(true);
			_nextHighestID = 0;

		}

		// add observer to container of observers
		public void Attach(T observer){

			// if available, use the ID number of a previously removed element
			int index = _pendingIDNumbers.Count > 0 ? _pendingIDNumbers.Dequeue() : _nextHighestID++;
			_observers[index] = observer;
			observer.SetID(index);

		}

		// remove observer from container of observers
		public void Detach(T observer){

			// get the ID of the given observer
			int index = observer.GetID();

			// if the observer is not contained, do not proceed
			if(!_observers.ContainsKey(index))
				return;

			// remove the observer at that index and enqueue its ID to be reused by a new observer
			_observers.Remove(index);
			_pendingIDNumbers.Enqueue(index);

		}

		// notify all observers of arguments
		public void NotifyAll(params string[] args){

			foreach(int index in _observers.Keys){

				Notify(_observers[index], args);

			}

		}

		// notify selected observer of arguments
		public abstract void Notify(T observer, params string[] args);

		// select observer to accept arguments
		public abstract T Select(params string[] args);

	}

	public interface IObserver<T> where T : IObserver<T> {

		public void SetID(int index);	// set ID of this element
		public int GetID();	// get ID of this element
		public void Update(params string[] args);

	}

	public abstract class BasicObserver : IObserver<BasicObserver> {

		private int _ID;

		public void SetID(int index){ _ID = index; }
		public int GetID() { return _ID; }
		public abstract void Update(params string[] args);

	}

}