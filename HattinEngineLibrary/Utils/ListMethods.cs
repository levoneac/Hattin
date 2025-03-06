namespace HattinEngineLibrary.Utils
{
    //Found out that this is the same as List<T>.intersect() :(
    public static class ListMethods
    {
        public static List<T> GetArrayOverlap<T>(List<T> array1, List<T> array2) where T : notnull
        {
            Dictionary<T, int> seen = new Dictionary<T, int>();
            List<T> values = new List<T>();

            foreach (T array1Elem in array1)
            {
                seen[array1Elem] = 1;
            }
            foreach (T array2Elem in array2)
            {
                if (seen.TryGetValue(array2Elem, out int count) && count == 1)
                {
                    values.Add(array2Elem);
                    seen[array2Elem] = 0;
                }
            }
            return values;
        }

    }

}