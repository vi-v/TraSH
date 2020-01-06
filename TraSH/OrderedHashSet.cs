// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

/* 
 * https://gmamaladze.wordpress.com/2013/07/25/hashset-that-preserves-insertion-order-or-net-implementation-of-linkedhashset/ 
 */

namespace TraSH
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public class OrderedHashSet<T> : ICollection<T>, IEnumerable<T>
    {
        private readonly IDictionary<T, LinkedListNode<T>> m_Dictionary;
        private readonly LinkedList<T> m_LinkedList;

        public OrderedHashSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        public OrderedHashSet(IEqualityComparer<T> comparer)
        {
            this.m_Dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
            this.m_LinkedList = new LinkedList<T>();
        }

        public int Count
        {
            get { return this.m_Dictionary.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return this.m_Dictionary.IsReadOnly; }
        }

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        public void Clear()
        {
            this.m_LinkedList.Clear();
            this.m_Dictionary.Clear();
        }

        public bool Remove(T item)
        {
            LinkedListNode<T> node;
            bool found = this.m_Dictionary.TryGetValue(item, out node);
            if (!found)
            {
                return false;
            }

            this.m_Dictionary.Remove(item);
            this.m_LinkedList.Remove(node);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.m_LinkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Contains(T item)
        {
            return this.m_Dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.m_LinkedList.CopyTo(array, arrayIndex);
        }

        public bool Add(T item)
        {
            if (this.m_Dictionary.ContainsKey(item))
            {
                return false;
            }

            LinkedListNode<T> node = this.m_LinkedList.AddLast(item);
            this.m_Dictionary.Add(item, node);
            return true;
        }
    }
}
