using System;
using System.Collections.Generic;
using AStarPathfinding;

namespace Pathfinding2D {

	[System.Serializable]
	class Node2D : Node<Node2D> {

		public int X { get; private set; }
		public int Y { get; private set; }

		private const int SQRT2 = 1;
		private PathConditions _conditions;

		public Node2D(int x, int y, PathConditions conditions) : this(x, y, conditions, null){ }

		public Node2D(int x, int y, PathConditions conditions, Node2D parent) : base(parent){

			X = x;
			Y = y;
			_conditions = conditions;

		}

		public override string ToString() { return "(" + X + ", " + Y + ")"; }

		// returns world index of node by calculating w/ local matrix coordinates
		public override int GetIndex(){

			return X * _conditions.SzX + Y;	// row-major order

		}
		
		// if the other node is not on the same row or column as this node, return false
		public bool IsDiagonal(Node2D to) {
			
			int dx = Math.Abs(X - to.X), dy = Math.Abs(Y - to.Y);
			return dx > 0 && dy > 0;

		}

		// returns the furthest distance along one axis
		//	this is more accurate for the heuristic than using manhattan distance
		public override float GetDistance(Node2D to) {

			// difference between coordinates
			int dx = Math.Abs(X - to.X), dy = Math.Abs(Y - to.Y);

			// return highest difference between coordinates
			//	if neighbor is diagonal to current, add sqrt(2)
			return (float)Math.Sqrt(dx * dx + dy * dy) + (IsDiagonal(to) ? SQRT2 : 0);

		}

		// returns accessible nodes adjacent to this node
		public override List<Node2D> GetNeighbors() {

			List<Node2D> neighbors = new List<Node2D>();

			// look at neighbors 1 spot away
			for(int dx = -1; dx <= 1; dx++){
				for(int dy = -1; dy <= 1; dy++){

					// skip this node's coordinates
					if(dx == 0 && dy == 0)
						continue;

					// create node
					Node2D neighbor = new Node2D(X + dx, Y + dy, _conditions, this);
					
					// if this node can be traveled to, add it to the list
					//	KEEP IN MIND THAT THE ALGORITHM SEARCHES FROM FINISH TO START POSITION
					// 	THEREFORE, IF YOU WANT TO SEE IF YOU CAN GO FROM TILE A TO TILE B
					//	YOU NEED TO CALL "_conditions.CanGo(B, A)" INSTEAD
					if(_conditions.CanGo(neighbor, this))
						neighbors.Add(neighbor);

				}
			}
			
			// return list of neighbors
			return neighbors;

		}

		// returns cost of traveling from this node to its neighbor
		public override int GetTravelCost(Node2D to){

			return _conditions.GetTravelCost(to);	// simply refer to _conditions

		}

		// returns true if coordinates are the same
		public override bool Equals(Node2D other){

			return X == other.X && Y == other.Y;

		}

		public override int GetHashCode(){
			int hash = 3;
			hash = hash * 5 + X.GetHashCode();
			hash = hash * 5 + Y.GetHashCode();
			return hash;
		}

	}

	[System.Serializable]
	class PathConditions {

		private int[,] _world;
		public int SzX { get; private set; }
		public int SzY { get; private set; }
		private bool _diagonalAllowed;

		public PathConditions(int[,] world, int szx, int szy, bool diagonalAllowed) {

			_world = world;
			SzX = szx;
			SzY = szy;
			_diagonalAllowed = diagonalAllowed;

		}

		public bool CanGo(Node2D from, Node2D to){

			// if out of bounds, return false
			if(IsOutOfBounds(to))
				return false;

			// if destination is diagonal from position
			if(from.IsDiagonal(to)){

				// if diagonal movement is not allowed, return false
				if(!_diagonalAllowed)
					return false;
					
				// get nodes of tiles adjacent to position AND destination
				Node2D xAdjacent = new Node2D(from.X, to.Y, this, from);
				Node2D yAdjacent = new Node2D(to.X, from.Y, this, from);

				// if cannot pass through both both adjacencies from position, return false
				if(!this.CanGo(from, xAdjacent) || !this.CanGo(from, yAdjacent))
					return false;
				
			}

			// simply return if tile value is 0, meaning walkable
			int tile = _world[to.X, to.Y];
			return tile == 0;

		}

		// cost of traveling between any tile is simply 1
		public int GetTravelCost(Node2D to){

			return 1;

		}

		// returns false if dimensions of world marix are exceeded
		private bool IsOutOfBounds(Node2D to){

			return to.X < 0 || to.X >= SzX || to.Y < 0 || to.Y >= SzY;

		}

	}

}