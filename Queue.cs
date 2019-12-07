namespace Queue {

	public interface IQueue<T> {

		void Add(T element);
		void Remove(T element);
		T Poll();
		T Peek();
		int Count { get; }

	}

}