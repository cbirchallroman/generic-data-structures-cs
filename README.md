# Generic Structures and Algorithms for C#

This is a repository for generic data structures and algorithms written in C#.

## Priority Queue

PriorityQueue<T> is an implementation of the priority queue structure that uses a max heap. To utilize in your program, use the namespace *PriorityQueue*.

```c#
using PriorityQueue;
```

The priority queue may be instantiated with a capacity and whether to reverse the order of the elements.

```c#
PriorityQueue<string> pq = new PriorityQueue<>();	// default capacity of 100
PriorityQueue<string> pq = new PriorityQueue<>(true);	// default capacity of 100, reverse order
PriorityQueue<string> pq = new PriorityQueue<>(200);	// instantiated with capacity of 200
PriorityQueue<string> pq = new PriorityQueue<>(200, true);	// instantiated with capacity of 200, reverse order
```

The following functionality is included:

```c#
void Enqueue(T element);	// adds element to the priority queue
void Remove(T element);	// removes element from the priority queue
void Update(T element);	// updates element's priority within the queue
T Dequeue();	// element from the top of the priority queue
T Peek();	// returns the element from the top of the priority queue without removing
bool Contains(T element);	// returns true if queue contains given element
```

## A* Pathfinding

The namespace *AStarPathfinding* contains tools to implement the A* pathfinding algorithm.

```c#
using AStarPathfinding;
```

First, extend the abstract class Node. The class is self-referential and extends the IComparable interface, so the header of the subclass will look like this:

```c#
class TestNode : Node<TestNode> { /*...*/ }
```

Under this subclass, override the following abstract functions, where N is the type of your subclass:

```c#
public abstract bool Equals(N other);	// checks if this node is equal to node of same type (ie. same coordinates)
public abstract int GetIndex();	// get world index of this node. it's easy to use the protected function GetIndex(int[]), that takes coordinates and world dimensions to do this. however, this function is O(n^d) where d is the number of dimensions. it's easier to make your own function that doesn't loop! the implementation in Pathfinding2D calls this function, but a hardcoded implementation is commented right above.
public abstract int GetTravelCost(N to);	// get cost of traveling to other node of same type
public abstract float GetDistance(N to);	// get distance from this node to other node of same type
public abstract List<N> GetNeighbors();	// get accessible nodes adjacent to this node
public override abstract int GetHashCode();	// get hashcode of node subclass
```

