using System.Collections;

namespace Hattin.Extensions
{
    //just for fun
    public static class GenericListExtensions
    {
        public static void AddRangeIfNotEmpty<T>(this List<T> list, List<T> data)
        {
            if (list is null)
            {
                throw new ArgumentException($"List cannot be null", nameof(list));
            }
            if (data is null)
            {
                throw new ArgumentException($"Data cant be null", nameof(data));
            }
            if (data.Count > 0)
            {
                list.AddRange(data);
            }
        }

        public static void AddIfNotEmpty<T>(this List<T> list, T data, int minCount = 0)
        {
            if (list is null)
            {
                throw new ArgumentException($"List cannot be null", nameof(list));
            }
            if (data is null)
            {
                throw new ArgumentException($"Data cant be null", nameof(data));
            }

            IList? array = data as IList;
            if (array is not null)
            {
                if (array.Count > minCount)
                {
                    list.Add(data);
                }
            }
        }
    }
}