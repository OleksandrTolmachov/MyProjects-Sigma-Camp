﻿using System.Collections;
using System.Reflection;
using Task_11.Products;

namespace Task_11.Storage
{
    public class Storage<T> : IStorage<T>, IStoragable, IEnumerable<T>
        where T : IProduct
    {
        private List<T> _products;

        public virtual event Action<T>? ProductAvaliableEvent;
        public virtual event Action<T>? ProductIsExpiredEvent;

        public int ProductAmount
        {
            get => _products.Count;
        }

        public Storage()
        {
            _products = new List<T>();
        }

        public Storage(params T[] products)
        {
            _products = new List<T>();
            foreach (var item in products)
            {
                if (!_products.Contains(item))
                    ProductAvaliableEvent?.Invoke(item);
                _products.Add(item);
                if (isExpired(item))
                {
                    ProductIsExpiredEvent?.Invoke(item);
                }
            }
        }

        public T this[int i]
        {
            get
            {
                if (i >= 0 && i < _products.Count)
                    return _products[i];
                throw new ArgumentException();
            }
        }

        //пошук по предикату
        public IEnumerable<T> Select(Func<T, bool> predicate)
        {
            foreach (var item in this)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        //універсальний пошук по предикату 
        public ICollection<T> FindWithPredicate(Func<PropertyInfo, bool> predicate)
        {
            List<T> list = new List<T>();
            try
            {
                foreach (var item in this)
                {
                    Type type = item.GetType();
                    foreach (PropertyInfo field in type.GetProperties())
                    {
                        if (predicate(field))
                        {
                            list.Add((T)item.Clone());
                        }
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return list;
        }

        public void AddItem(object item)
        {
            if (item == null)
            {
                return;
            }
            else if (!_products.Contains((T)item))
            {
                ProductAvaliableEvent?.Invoke((T)item);
            }
            _products.Add((T)item);

            if (isExpired((T)item))
            {
                ProductIsExpiredEvent?.Invoke((T)item);
            }
        }

        private bool isExpired(T? item)
        {
            if (item is IExpirableProduct expirableProduct
               && expirableProduct.TimeToExpire < DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public void RemoveItem(object item)
        {
            if (item != null)
            {
                _products.Remove((T)item);
            }
        }

        public bool TryToFindSpecialType<Type>(out IEnumerable<T> products)
        {
            products = _products.Where(x => x is Type);
            return products.Count() > 0;
        }

        public void ChangePriceOfAllProducts(int percentage)
        {
            foreach (var item in _products)
            {
                item.ChangePrice(percentage);
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Storage<T> storage &&
                   EqualityComparer<List<T>>.Default.Equals(_products, storage._products);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_products);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _products.Count; i++)
            {
                yield return (T)_products[i].Clone();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}