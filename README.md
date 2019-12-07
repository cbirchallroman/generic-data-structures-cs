# Generic Data Structures for C#

This is a repository for generic data structures written in C#.

## Priority Queue

PriorityQueue<T> is an implementation of the priority queue structure that uses a max heap. To utilize in your program, use the namespace *PriorityQueue*.

```c#
using PriorityQueue;
```

The following functionality is included:

```c#
void Add(T element);	// adds element to the priority queue
T Poll();	// element from the top of the priority queue
T Peek();	// returns the element from the top of the priority queue without removing
```
