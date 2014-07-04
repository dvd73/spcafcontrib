using System;

namespace SPCAFContrib.Test.TryCatch
{
    public static class BadTryCatchImplExtensions
    {
        #region methods

        public static void WithProperExceptionHandling(this BadTryCatchImpl instance, Action action)
        {
            try
            {
                action();
            }
// ReSharper disable RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
// ReSharper restore RedundantCatchClause
        }

        public static void WithNotProperExceptionHandling(this BadTryCatchImpl instance, Action action)
        {
            try
            {
                action();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        #endregion
    }
}
