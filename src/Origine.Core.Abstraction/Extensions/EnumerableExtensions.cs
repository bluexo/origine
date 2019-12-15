using System.Linq;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 找到集合内的随机元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collections"></param>
        /// <returns></returns>
        public static T Random<T>(this IEnumerable<T> collections)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            if (collections == null)
                return default;
            T[] array = null;
            if (collections is T[] arr)
                array = arr;
            if (array == null)
                array = collections.ToArray();
            return array.Length > 0
                ? array[random.Next(array.Length)]
                : default;
        }

        /// <summary>
        /// 打乱一个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        private static void ShuffleSelf<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0) return;

            var newList = list.Select(c => (color: c, id: Guid.NewGuid()))
                .OrderBy(t => t.id)
                .Select(t => t.color)
                .ToList();

            list.Clear();
            foreach (var e in newList) list.Add(e);
        }

        /// <summary>
        /// 根据一个有序集合返回另一个打乱后的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collections"></param>
        /// <returns></returns>
        public static List<T> Shuffle<T>(this IEnumerable<T> collections)
        {
            if (collections == null) return null;
            var list = collections.ToList();
            list.ShuffleSelf();
            return list;
        }
        
        public static string ToArrayString<T>(this IEnumerable<T> collection, char separator = ' ')
        {
            var str = string.Empty;
            foreach (var item in collection) str += item.ToString() + separator;
            return str;
        }

        public static string ToArrayString<T>(this IEnumerable<T> collection, Func<T, string> toString, char separator = ' ')
        {
            var str = string.Empty;
            foreach (var item in collection) str += toString(item) + separator;
            return str;
        }

        /// <summary>
        /// 平铺数值 , 将一组数字列表按照给定数量平铺到另一个集合中，根据每个元素的大小作为权重得出符合每个元素的公平的数量
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="collection"></param>
        /// <param name="amount"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static IDictionary<TValue, int> Spread<TValue>(this IList<TValue> collection, int amount, float factor = 0f)
            where TValue : struct
        {
            if (!typeof(TValue).IsPrimitive)
                throw new InvalidCastException($"{nameof(Spread)} cannot support {typeof(TValue).FullName} , please convert to Primitive C# type!");
            amount = Math.Max(collection.Count, amount);
            var amountfactor = amount / collection.Select(n => amount / Convert.ToDouble(n)).Sum() + factor;
            var result = collection
                .Select(n => (number: n, count: (int)(amount / Convert.ToDouble(n) * amountfactor)))
                .GroupBy(g => g.number)
                .ToDictionary(g => g.Key, g => g.Select(c => c.count).Sum());
            return result;
        }

        /// <summary>
        /// 范围随机 , 给定一组数字，按照数字的大小来决定该数字出现的概率, 然后返回数字的索引
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int RandomScope<TValue>(this IList<TValue> array)
        {
            var random = new Random();
            var randomValue = random.Next((int)(array.Sum(r => Convert.ToDouble(r))));
            var randomIndex = 0;
            for (var i = 0; i < array.Count; i++)
            {
                var max = array.Take(i + 1).Sum(r => Convert.ToDouble(r));
                var min = i == 0 ? 0 : array.Take(i).Sum(r => Convert.ToDouble(r));
                if (randomValue < max && randomValue >= min)
                {
                    randomIndex = i;
                    break;
                }
            }
            return randomIndex;
        }
    }
}
