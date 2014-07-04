using System;
using System.Collections.Generic;
using System.Text;

namespace SharePoint.Common.Utilities.Extensions
{
    public static class IEnumerableExtension
    {
        public static string ConcatenateIntoString<T>(this IEnumerable<T> listOfValues, string delimiter, bool addDelimiterAfterLastItem, bool addDelimiterBeforeFirstItem, Func<T, string> transformIntoString)
        {
            if (listOfValues == null) return null;
            var sb = new StringBuilder();
            int counter = 0;
            foreach (T value in listOfValues)
            {
                counter++;
                if (counter > 1 || addDelimiterBeforeFirstItem) // Append delimetere if (1) current item is the first item (i.e. counter == 1 ) and addDelimeterBeforeFirstItem or (2) it is not first item.
                    sb.Append(delimiter);
                sb.Append(transformIntoString(value));
            }
            if (addDelimiterAfterLastItem) sb.Append(delimiter);
            return sb.ToString();
        }

        public static string ConcatenateIntoString<T>(this IEnumerable<T> listOfValues, string delimiter)
        {
            return ConcatenateIntoString(listOfValues, delimiter, false, false, (item) => item.ToString());
        }

        /// <summary>  
        /// This method adds the items in the first list to the second list.  
        /// This method is  helpful because even when X inherits from Y, List<X> does not inherit from List<Y>, and so you cannot cast between them, only copy.  
        /// </summary>  
        /// <typeparam name="FROM_TYPE"></typeparam>  
        /// <typeparam name="TO_TYPE"></typeparam>  
        /// <param name="listToCopyFrom"></param>  
        /// <param name="listToCopyTo"></param>  
        /// <returns></returns>  
        public static List<TO_TYPE> AddRange<FROM_TYPE, TO_TYPE>(this IEnumerable<FROM_TYPE> listToCopyFrom, List<TO_TYPE> listToCopyTo) where FROM_TYPE : TO_TYPE
        {
            // loop through the list to copy, and  
            foreach (FROM_TYPE item in listToCopyFrom)
            {
                // add items to the copy tolist  
                listToCopyTo.Add(item);
            }

            // return the copy to list  
            return listToCopyTo;
        }  
    }
}
