using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Utilities
{
    public static class AppUtilities
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Tìm số bị thiếu trong mảng
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static int FindMissingNumber(int[] arr)
        {
            // Tìm số còn thiếu trong mảng
            int missingNumber = 0;
            // Phần tử đầu tiên cần check là 1
            int firstIndex = 1;
            // Phần tử cuối trong mảng
            int lastIndex = 1;
            if (arr != null && arr.Any())
                lastIndex = arr.OrderByDescending(e => e).FirstOrDefault();
            missingNumber = Enumerable.Range(firstIndex, lastIndex).Except(arr).FirstOrDefault();
            return missingNumber;
        }

    }
}
