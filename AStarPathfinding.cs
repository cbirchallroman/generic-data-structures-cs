using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using PriorityQueue;

namespace AStarPathfinding {

	public enum Algorithm { AStar, Dijkstra, Greedy }

	public class Pathfinder<N> where N : Node<N> {

		private int[] _dimensions;
		public Algorithm Type { get; private set; }

		// derivatives of Node class will exist on the same plane as the Pathfinder that accesses them
		public Pathfinder(int[] dimensions) : this(dimensions, Algorithm.AStar){ }
		public Pathfinder(int[] dimensions, Algorithm type){

			_dimensions = dimensions;
			Type = type;

		}

		private int GetArea(){

			int product = 0;
			foreach(int d in _dimensions){
				if(product == 0)
					product = d;
				else if(d != 0)
					product *= d;
			}

			return product;

		}

		public Queue<N> FindPath(N from, N to){

			PriorityQueue<N> queue = new PriorityQueue<N>();
			to.SetGoal(from);
			to.SetType(Type);
			queue.Enqueue(to);

			return FindPathHelper(from, queue);

		}

		public Queue<N> FindPath(N from, List<N> toList){

			// if no ending nodes, return null
			if(toList.Count == 0)
				return null;

			PriorityQueue<N> queue = new PriorityQueue<N>();	// order in which nodes will be searched

			// for each possible exit, set weight to 0 and have no edge to previous node
			foreach(N n in toList){

				n.SetGoal(from);
				n.SetType(Type);
				queue.Enqueue(n);

			}

			// return null if no path found
			return FindPathHelper(from, queue);

		}

		private Queue<N> FindPathHelper(N goal, PriorityQueue<N> queue){

			// using a dictionary because access is O(n)
			Dictionary<int, float> weights = new Dictionary<int, float>();
			Dictionary<int, bool> visited = new Dictionary<int, bool>();

			// while there are still unvisited nodes enqueued
			while(queue.Count > 0){

				N current = queue.Dequeue();

				// mark current node as visited
				visited[current.GetIndex()] = true;

				// if this is the goal, iterate through nodes to construct the path
				if(current.Equals(goal)){

					return current.RetracePath();

				}

				// enqueue neighbors
				foreach(N neighbor in current.GetNeighbors()){

					// if this spot is visited, continue
					//	visited will never contain any false values,
					//	so referring to visited.ContainsKey(int) is a safe bet
					if(visited.ContainsKey(neighbor.GetIndex()))
						continue;

					// if queue contains neighbor already, replace with new object if weight is smaller
					//	otherwise don't consider any further
					if(weights.ContainsKey(neighbor.GetIndex())){

						float originalWeight = weights[neighbor.GetIndex()];
						if(neighbor.Weight < originalWeight)
							queue.Remove(neighbor);
						else
							continue;

					}

					// if we're past that, update weights[] and enqueue node
					weights[neighbor.GetIndex()] = neighbor.Weight;
					queue.Enqueue(neighbor);

				}

			}

			return null;

		}

	}

	[System.Serializable]
	public abstract class Node<N> : IComparable<N> where N : Node<N>  {

		protected N Goal { get; private set; }	// final destination node
		protected N Parent{ get; private set; }	// previous node from the pathfinder
		protected Algorithm Type { get; private set; }	// algorithm according to which priority (weight) is determined
		private float _distanceTraveled;
		private float _heuristic;

		/* CONSTRUCTORS */
		public Node(){ }

		public void SetParent(N parent){

			Parent = parent;
			if(Parent == null)
				_distanceTraveled = 0;
			else{
				_distanceTraveled = Parent._distanceTraveled + Parent.GetTravelCost((N)this);
				SetGoal(Parent.Goal);
				SetType(Parent.Type);
			}

		}

		public void SetGoal(N goal){

			Goal = goal;
			if(Goal == null)
				_heuristic = 0;
			else
				_heuristic = GetDistance(Goal);

		}

		public void SetType(Algorithm type){

			Type = type;

		}

		/* IMPLEMENTED METHODS */
		// this function allows the pathfinding algorithm to get the next available node
		//	in general, it allows the user to sort nodes together based on relative weight in a priority queue
		//	thus a priorityqueue of nodes set to "greedy" type can be used to get the node closest to some given point
		public int CompareTo(N obj){ return (int)(obj.Weight - this.Weight); }

		// returns node priority depending upon selected algorithm
		public float Weight { get {

			switch(Type){
				case Algorithm.Dijkstra:
					return _distanceTraveled;
				case Algorithm.Greedy:
					return _heuristic;
				default:
					return _distanceTraveled + _heuristic;
			}

		} }

		public override bool Equals(Object obj){

			if(obj.GetType() != GetType())
				return false;
			
			N other = (N)obj;
			return Equals(other);

		}

		// returns index of node based on coordinates and world size
		protected int GetIndex(int[] coordinates, int[] worldSize){

			int rank = worldSize.Length;	// the number of dimensions

			// if the coordinates and world size have different dimensions, return -1
			if(coordinates.Length != rank)
				return -1;
			

			int factor = 1;	// multiplication factor begins at 1
			int index = coordinates[rank - 1]; // the last component is added without multiplying it first

			for(int dimension = rank - 2; dimension >= 0; dimension--){

				factor *= worldSize[dimension + 1];	// multiply factor by previous size dimension
				index += coordinates[dimension] * factor;	// add component times factor

			}

			return index;

		}

		public Queue<N> RetracePath(){

			// create stack of nodes
			Queue<N> path = new Queue<N>();

			// point 'current' to start, and don't push this node because it won't be included in the path
			N current = (N)this;

			// while current has a parent, set to parent and push to path
			while(current.Parent != null){
				current = current.Parent;
				path.Enqueue(current);
			}

			return path;

		}
		
		/* ABSTRACT METHODS */
		public abstract bool Equals(N other);	// checks if this node is equal to node of same type
		public abstract int GetIndex();	// get world index of this node
		public abstract float GetTravelCost(N to);	// get cost of traveling to other node of same type
		public abstract float GetDistance(N to);	// get distance from this node to other node of same type
		public abstract List<N> GetNeighbors();	// get accessible nodes adjacent to this node
		public override abstract int GetHashCode();	// get hashcode of node subclass

	}

}