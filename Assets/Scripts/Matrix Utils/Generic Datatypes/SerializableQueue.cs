using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableQueue<T> : IReadOnlyCollection<T>
{
    Queue<T> m_underlyingQueue = new();
    [SerializeField] List<T> m_queue = new();
    public SerializableQueue() { }
    
    public SerializableQueue(int capacity)
    {
        m_underlyingQueue = new Queue<T>(capacity);
    }
    
    public SerializableQueue(IEnumerable<T> collection)
    {
        m_underlyingQueue = new Queue<T>(collection);
    }

    public int Count => m_underlyingQueue.Count;
    public void Enqueue(T item) => m_underlyingQueue.Enqueue(item);
    public T Dequeue() => m_underlyingQueue.Dequeue();
    public T Peek() => m_underlyingQueue.Peek();
    public bool TryDequeue(out T result) => m_underlyingQueue.TryDequeue(out result);
    public bool TryPeek(out T result) => m_underlyingQueue.TryPeek(out result);
    public void Clear() => m_underlyingQueue.Clear();
    public bool Contains(T item) => m_underlyingQueue.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => m_underlyingQueue.CopyTo(array, arrayIndex);
    public T[] ToArray() => m_underlyingQueue.ToArray();
    public void TrimExcess() => m_underlyingQueue.TrimExcess();
    public IEnumerator<T> GetEnumerator() => m_underlyingQueue.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => m_underlyingQueue.GetEnumerator();

    public void OnBeforeSerialize()
    {
        m_queue.Clear();
        foreach (T item in m_underlyingQueue)
        {
            m_queue.Add(item);
        }
    }
    
    public void OnAfterDeserialize()
    {
        m_underlyingQueue.Clear();
        foreach (T item in m_queue)
        {
            m_underlyingQueue.Enqueue(item);
        }
    }
}