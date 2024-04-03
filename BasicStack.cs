namespace TravelApp;

internal class BasicStack<T>
{
    private readonly T[] _items;

    private int _currentIndex = -1;

    public BasicStack() => _items = new T[10];

    public int Count => _currentIndex + 1;

    public void Push(T item) => _items[++_currentIndex] = item;

    public T Pop() => _items[_currentIndex--];
}