# Generic Data Structures for C#

This is a repository for generic data structures written in C#.

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
