using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using PriorityQueue;

namespace AStarPathfinding {

	public class Pathfinder<N> where N : Node<N> {

		private int[] _dimensions;

		// derivatives of Node class will exist on the same plane as the Pathfinder that accesses them
		public Pathfinder(int[] dimensions){

			_dimensions = dimensions;

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

		public Queue<N> FindPath(N start, List<N> ends){

			// if no ending nodes, return null
			if(ends.Count == 0)
				return null;

			PriorityQueue<N> queue = new PriorityQueue<N>();	// order in which nodes will be searched

			// using a dictionary because access is O(n)
			Dictionary<int, float> weights = new Dictionary<int, float>();
			Dictionary<int, bool> visited = new Dictionary<int, bool>();

			// we are doing the algorithm in reverse
			//	that way the user may search for paths to multiple exits
			N goal = start;
			foreach(N n in ends){

				// for each possible exit, set weight to 0 and have no edge to previous node
				n.SetWeight(0, 0, 0);
				queue.Enqueue(n);

			}

			// while there are still unvisited nodes enqueued
			while(queue.Count > 0){

				N current = queue.Dequeue();

				// if this is the goal, iterate through nodes to construct the path
				if(current.Equals(goal)){

					// use the parent as the argument because we don't want to include the current node
					return RetracePath(current.Parent);

				}

				// mark current node as visited
				visited[current.GetIndex()] = true;

				// enqueue neighbors
				foreach(N neighbor in current.GetNeighbors()){

					// if this spot is visited, continue
					//	visited will never contain any false values,
					//	so referring to visited.ContainsKey(int) is a safe bet
					if(visited.ContainsKey(neighbor.GetIndex()))
						continue;

					// f = total priority weight of neighbor
					//	f = g + h
					//	g = distance traveled up to current + cost of traveling through neighbor
					//	h = distance from neighbor to goal
					int distanceTraveled = current.DistanceTraveled;
					int travelScore = current.GetTravelCost(neighbor);
					float heuristic = neighbor.GetDistance(goal);
					
					// set weight (f) of neighbor
					neighbor.SetWeight(distanceTraveled, travelScore, heuristic);

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

			// return null if no path found
			return null;

		}

		public Queue<N> RetracePath(N start){

			// create stack of nodes
			Queue<N> path = new Queue<N>();

			// point 'current' to start, push current to path
			N current = start;
			path.Enqueue(current);

			// while current has a parent, set to parent and push to path
			while(current.Parent != null){
				current = current.Parent;
				path.Enqueue(current);
			}

			return path;

		}

	}

	[System.Serializable]
	public abstract class Node<N> : IComparable<N> where N : Node<N>  {

		public N Parent{ get; private set; }	// previous node from the pathfinder
		public int DistanceTraveled { get; private set; }
		private int _travelScore;
		private float _heuristic;
		public float Weight { get { return DistanceTraveled + _travelScore + _heuristic; } }

		/* CONSTRUCTORS */
		public Node() { Parent = null; }
		public Node(N parent){ Parent = parent; }

		/* IMPLEMENTED METHODS */
		public int CompareTo(N obj){ return (int)(this.Weight - obj.Weight); }
		public override bool Equals(Object obj){

			if(obj.GetType() != GetType())
				return false;
			
			N other = (N)obj;
			return Equals(other);

		}

		/* FINAL METHODS */
		public void SetWeight(int distanceTraveled, int travelScore, float heuristic){

			DistanceTraveled = distanceTraveled;
			_travelScore = travelScore;
			_heuristic = heuristic;

		}
		
		/* ABSTRACT METHODS */
		public abstract bool Equals(N other);	// checks if this node is equal to node of same type
		public abstract int GetIndex();	// get world index of this node
		public abstract int GetTravelCost(N to);	// get cost of traveling to other node of same type
		public abstract float GetDistance(N to);	// get distance from this node to other node of same type
		public abstract List<N> GetNeighbors();	// get accessible nodes adjacent to this node
		public override abstract int GetHashCode();	// get hashcode of node subclass

	}

}