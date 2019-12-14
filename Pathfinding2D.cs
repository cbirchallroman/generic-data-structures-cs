using System;
using System.Collections.Generic;
using AStarPathfinding;

namespace Pathfinding2D {

	public class Pathfinder2D<W> where W : World<W>{

		private Pathfinder<Node2D<W>> _pathfinder;
		private Algorithm _type;

		public Pathfinder2D() : this(Algorithm.AStar) { }
		public Pathfinder2D(Algorithm type){
			_pathfinder = new Pathfinder<Node2D<W>>();
			_type = type;
		}

		public Queue<Node2D<W>> FindPath(Node2D<W> start, Node2D<W> finish, PathConditions<W> conditions){

			start.SetTravelConditions(conditions);
			finish.SetTravelConditions(conditions);

			return _pathfinder.FindPath(start, finish, _type);

		}

	}

	[System.Serializable]
	public class Node2D<W> : Node<Node2D<W>> where W : World<W> {

		public int X { get; private set; }
		public int Y { get; private set; }

		private const float SQRT2 = 1.4f;
		private PathConditions<W> _conditions;
		private W _world;

		public Node2D(int x, int y, W world){

			X = x;
			Y = y;
			_world = world;

		}

		public void SetTravelConditions(PathConditions<W> conditions){

			_conditions = conditions;

		}

		public override string ToString() { return "(" + X + ", " + Y + ")"; }

		// returns unique index of node with local matrix coordinates
		public override int GetIndex(){

			//int index = X * (_conditions.SzX - 1) + Y;
			int index = GenerateIndex(new int[]{X, Y}, _world.Dimensions);
			return index;

		}
		
		// if the other node is not on the same row or column as this node, return false
		public bool IsDiagonal(Node2D<W> to) {
			
			int dx = Math.Abs(X - to.X), dy = Math.Abs(Y - to.Y);
			return dx > 0 && dy > 0;

		}

		// returns the furthest distance along one axis
		//	this is more accurate for the heuristic than using manhattan distance
		public override float GetDistance(Node2D<W> to) {

			// difference between coordinates
			int dx = Math.Abs(X - to.X);
			int dy = Math.Abs(Y - to.Y);

			// octile distance if diagonal movement allowed
			if(_conditions?.DiagonalAllowed != null)
				return (dx + dy) + (SQRT2 - 2) * (Math.Min(dx, dy));

			// manhattan distance: simply add differences along both axes
			return dx + dy;

		}

		// returns accessible nodes adjacent to this node
		public override List<Node2D<W>> GetNeighbors() {

			List<Node2D<W>> neighbors = new List<Node2D<W>>();

			// look at neighbors 1 spot away
			for(int dx = -1; dx <= 1; dx++){
				for(int dy = -1; dy <= 1; dy++){

					// skip this node's coordinates
					if(dx == 0 && dy == 0)
						continue;

					// create node
					Node2D<W> neighbor = new Node2D<W>(X + dx, Y + dy, _world);
					
					// if this node can be traveled to, add it to the list
					//	KEEP IN MIND THAT THE ALGORITHM SEARCHES FROM FINISH TO START POSITION
					// 	THEREFORE, IF YOU WANT TO SEE IF YOU CAN GO FROM TILE A TO TILE B
					//	YOU NEED TO CALL "_conditions.CanGo(B, A)" INSTEAD
					if(_conditions.CanGo(neighbor, this)){
						neighbor.SetTravelConditions(_conditions);
						neighbor.SetParent(this);
						neighbors.Add(neighbor);
					}

				}
			}
			
			// return list of neighbors
			return neighbors;

		}

		// returns cost of traveling from this node to its neighbor
		public override float GetTravelCost(Node2D<W> to){

			float cost = _world.GetTravelCost(to);	// simply get from _world
			cost *= IsDiagonal(to) ? SQRT2 : 1;	// multiply by 1.4 if diagonal
			return cost;

		}

		public override int GetHashCode(){
			int hash = 3;
			hash = hash * 5 + X.GetHashCode();
			hash = hash * 5 + Y.GetHashCode();
			return hash;
		}

	}

	[System.Serializable]
	public abstract class World<W> where W : World<W>{

		public int SizeX { get; protected set; }
		public int SizeY { get; protected set; }
		public int[] Dimensions { get { return new int[]{SizeX, SizeY}; } }
		
		public bool IsOutOfBounds(Node2D<W> node){

			return node.X < 0 || node.X >= SizeX || node.Y < 0 || node.Y >= SizeY;

		}

		public abstract int GetTravelCost(Node2D<W> node);

	}

	[System.Serializable]
	public abstract class PathConditions<W> where W : World<W> {

		protected W _world;
		public bool DiagonalAllowed { get; protected set; }

		public PathConditions(W world, bool diagonalAllowed) {

			_world = world;
			DiagonalAllowed = diagonalAllowed;

		}

		public bool CanGo(Node2D<W> from, Node2D<W> to){

			// if out of bounds, return false
			if(_world.IsOutOfBounds(to) || _world.IsOutOfBounds(from))
				return false;

			// if destination is diagonal from position
			if(from.IsDiagonal(to)){

				// if diagonal movement is not allowed, return false
				if(!DiagonalAllowed)
					return false;
					
				// get nodes of tiles adjacent to position AND destination
				Node2D<W> xAdjacent = new Node2D<W>(from.X, to.Y, _world);
				Node2D<W> yAdjacent = new Node2D<W>(to.X, from.Y, _world);

				// if cannot pass through both both adjacencies from position, return false
				if(!CanGo(from, xAdjacent) || !CanGo(from, yAdjacent))
					return false;
				
			}

			// determine with implementation-specific function from here
			return CanGoConditions(from, to);

		}

		protected abstract bool CanGoConditions(Node2D<W> from, Node2D<W> to);

	}

}