# Generic Data Structures for C#

This is a repository for generic data structures written in C#.

## Priority Queue

PriorityQueue<T> is an implementation of the priority queue structure that uses a max heap. To utilize in your program, use the namespace *PriorityQueue*.

```c#
using PriorityQueue;
```

The following functionality is included:

```c#
void Enqueue(T element);	// adds element to the priority queue
void Remove(T element);	// removes element from the priority queue
void Update(T element);	// updates element's priority within the queue
T Dequeue();	// element from the top of the priority queue
T Peek();	// returns the element from the top of the priority queue without removing
```
