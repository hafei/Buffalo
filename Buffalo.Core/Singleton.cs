using System;
using System.Collections.Generic;
using System.Text;

namespace Buffalo.Core
{
    public class Singleton
    {
        static Singleton()
        {
            allSingletons = new Dictionary<Type, object>();
        }

        private static readonly IDictionary<Type, object> allSingletons;

        public static IDictionary<Type, object> AllSingletons
        {
            get { return allSingletons; }
        }
    }



    public class Singleton<T> : Singleton
    {
        private static T instance;

        public static T Instance
        {
            get { return instance; }
            set
            {
                instance = value;
                AllSingletons[typeof(T)] = value;
            }
        }
    }

    public class SingletonList<T> : Singleton<IList<T>>
    {
        static SingletonList()
        {
            Singleton<IList<T>>.Instance = new List<T>();
        }

        public new static IList<T> Instance
        {
            get { return Singleton<IList<T>>.Instance; }
        }
    }


    public class SingleDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
    {
        static SingleDictionary()
        {
            Singleton<IDictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
        }

        public new static IDictionary<TKey, TValue> Instance
        {
            get { return Singleton<Dictionary<TKey, TValue>>.Instance; }
        }
    }
}
