namespace Queue {

	public interface IQueue<T> {

		void Enqueue(T element);
		void Remove(T element);
		T Dequeue();
		T Peek();
		int Count { get; }

	}

}