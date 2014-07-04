using System;
using System.Reflection;
using SPCAF.Sdk.Model;

namespace SPCAFContrib.Extensions
{
    public static class FeatureDefinitionExtensions
    {
        #region methods

        public static FeatureDefinition FindParentFeature<T>(this IChildItem<T> item)
        {
            return FindParentDefinition<T, FeatureDefinition>(item);
        }

        public static V FindParentDefinition<T,V>(IChildItem<T> item) where V:class
        {
            object tmpItem = item;

            while (tmpItem != null && !(tmpItem is V))
            {
                Type type = tmpItem.GetType();
                PropertyInfo prop = type.GetProperty("Parent");

                if (prop == null) return null;

                tmpItem = prop.GetValue(tmpItem, null);
            }

            return tmpItem as V;
        }

        #endregion
    }
}
