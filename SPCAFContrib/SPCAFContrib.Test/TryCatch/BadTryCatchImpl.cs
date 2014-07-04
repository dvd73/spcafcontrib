using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SPCAFContrib.Test.TryCatch
{
    public class BadTryCatchImpl
    {
        #region utils

        private void Call()
        {

        }

        private void Log()
        {

        }

        #endregion

        public class Tmp : IDisposable
        {
            public List<int> Test { get; set; }

            public List<int> GetList()
            {
                return Test;
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        public void LoadCustomersData(Tmp tmp)
        {
            Console.WriteLine();

            foreach (var list in tmp.GetList())
            {
                Console.WriteLine(list);
            }
        }

        #region neutral methods

        public static void Neutral_Using_Static_StreamWriter_WithoutTryCatch(string msg)
        {
            using (var write = new StreamWriter(@"c:\test\1.txt", true))
            {
                write.WriteLine(msg);
            }
        }

        public void Neutral_Using_StreamWriter_WithoutTryCatch(string msg)
        {
            using (var write = new StreamWriter(@"c:\test\1.txt", true))
            {
                write.WriteLine(msg);
            }
        }


        public void Neutral_Using_Auto_WithoutTryCatch(string data)
        {
            using (var streamReader = new StreamReader(""))
            {
                var a = streamReader.ReadToEnd();
                Console.WriteLine(a);
            }
        }



        public void Neutral_Using_Auto_External_WithoutTryCatch(string data)
        {
            StreamReader streamReader;

            using (streamReader = new StreamReader(""))
            {
                var a = streamReader.ReadToEnd();
                Console.WriteLine(a);
            }
        }


        public int Neutral_Using_WithMethod_WithoutTryCatch(String licenseTerms)
        {
            using (var ms = new MemoryStream(Convert.FromBase64String(licenseTerms)))
            {
                var bnfmt = new BinaryFormatter();
                var value = bnfmt.Deserialize(ms);

                if (value is int)
                {
                    return (int) value;
                }
// ReSharper disable RedundantIfElseBlock
                else
// ReSharper restore RedundantIfElseBlock
                {
                    throw new ApplicationException("Invalid Type!");
                }
            }
        }

        public void Negative_Using_Manual_WithoutTryCatch(string data)
        {
            StreamReader streamReader = null;

            try
            {
                streamReader = new StreamReader("");
                var a = streamReader.ReadToEnd();
                Console.WriteLine(a);
            }
            finally
            {
                if (streamReader != null)
                    ((IDisposable)streamReader).Dispose();
            }

        }

        public void Neutral_Using_In_Using_In_Using_WithoutTryCatch(string data)
        {
            using (var tt = new Tmp())
            {
                Log();

                using (var yy = new Tmp())
                {
                    using (var zz = new Tmp())
                    {
                    }
                }

                Call();

                using (var zz = new Tmp())
                {

                }

                Log();
            }
        }

        public void Neutral_Using_In_Double_Using_WithoutTryCatch(string data)
        {
            using (var tt = new Tmp())
            {
                Log();

                using (var yy = new Tmp())
                {

                }

                Call();

                using (var zz = new Tmp())
                {

                }

                Log();
            }
        }

        public void Neutral_Using_In_Using_WithoutTryCatch(string data)
        {
            using (var tt = new Tmp())
            {
                using (var yy = new Tmp())
                {

                }
            }
        }

        public void Neutral_Using_Custom_WithoutTryCatch(string data)
        {
            using (var tt = new Tmp())
            {

            }
        }

        public void Neutral_Using_Double_Custom_WithoutTryCatch(string data)
        {
            using (var tt = new Tmp())
            {

            }

            Call();

            using (var tt = new Tmp())
            {

            }
        }

        public void Neutral_While_WithoutTryCatch(string data)
        {
            var testTypes = new Mono.Collections.Generic.Collection<string>();
            var ct = string.Empty;

            int count = 0;

            while (ct != null)
            {
                if (count < 10)
                {
                    try
                    {
                        System.Threading.Thread.Sleep(5000);
                        ct = testTypes[count];

                    }
#pragma warning disable 168
                    catch (Exception e)
                    {
                        Neutral_While_WithoutTryCatch("");
                    }
                    finally
                    {
                        count++;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public void Neutral_Foreach_WithoutTryCatch()
        {
            var items = new List<int>();

            foreach (var item in items)
            {

            }
        }

        public void Neutral_EmptyMethod()
        {

        }

        public static void Neutral_EmptyStaticMethod()
        {

        }

        public void Neutral_Empty_ZeroMethodCall()
        {

        }

        public void Neutral_Empty_OneMethodCall()
        {
            Call();
        }

        public void Neutral_Empty_TwoMethodCall()
        {
            Call();
            Log();
        }

        public void Neutral_Empty_ThreeMethodCall()
        {
            Call();
            Log();
            Call();
        }

        #endregion

        #region negative

        public void Negative_TryBody_NoCatch_FinallyEmpty()
        {
            try
            {
                Call();
            }
// ReSharper disable RedundantEmptyFinallyBlock
            finally
            {

            }
// ReSharper restore RedundantEmptyFinallyBlock
        }

        public void Negative_TryBody_CatchEmpty()
        {
            try
            {
                Call();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {

            }
        }

        public void Negative_TryBody_CatchEmptyWithExceptionHandling()
        {
            try
            {
                Call();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception e)
// ReSharper restore EmptyGeneralCatchClause
            {

            }
        }

        public void Negative_TryBody_CatchBody_FirstCallNotWrapped()
        {
            Call();

            try
            {
                Call();
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        public void Negative_TryBody_CatchBody_LastCallNotWrapped()
        {
            try
            {
                Call();
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }

            Call();
        }

        public void Negative_TryBody_CatchBody_FirstAndLastCallNotWrapped()
        {
            Call();

            try
            {
                Call();
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }

            Call();
        }

        public void Negative_TryBody_CatchBody_FirstEnumeratorNotWrapped()
        {
            for (var i = 0; i < 1; i++)
            {
                var t = i + 1;
            }

            try
            {
                Call();
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        #endregion

        #region positive

        public void Positive_TryBody_CatchBody()
        {
            try
            {
                Call();
            }
            catch (Exception e)
            {
                Log();
            }
        }

        public void Positive_TryBody_CatchBodyWithThrow()
        {
            try
            {
                Call();
            }
// ReSharper disable RedundantCatchClause
            catch (Exception e)
            {
                throw;
            }
// ReSharper restore RedundantCatchClause
        }

        public void Positive_TryBody_CatchBodyWithThrowNewExcption()
        {
            try
            {
                Call();
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        public void Positive_TryBody_CatchBody_FullyWrappedMethod()
        {
            try
            {
                Call();
                Call();
                Call();
            }
// ReSharper disable RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
// ReSharper restore RedundantCatchClause
        }

        #endregion

        #region try-catch helpers

        public void FirstLevel_TryCatch_Helper()
        {
            try
            {
                Call();
                Log();
            }
// ReSharper disable RedundantCatchClause
            catch (Exception)
            {
                throw;
            }
// ReSharper restore RedundantCatchClause
        }

        public void FirstLevel_Empty_TryCatch_Helper()
        {
            try
            {
                Call();
                Log();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {

            }
        }

        public void Positive_FirstLevel_TryCatch_Helper()
        {
            FirstLevel_TryCatch_Helper();
        }

        public void Negative_FirstLevel_TryCatch_Helper()
        {
            FirstLevel_Empty_TryCatch_Helper();
        }

        public void Positive_FirstLevel_TryCatch_Extension_Helper()
        {
            this.WithProperExceptionHandling(() =>
            {
                FirstLevel_TryCatch_Helper();
            });
        }

        public void Negative_FirstLevel_TryCatch_Extension_Helper()
        {
            this.WithNotProperExceptionHandling(() =>
            {
                FirstLevel_TryCatch_Helper();
            });
        }

        #endregion
    }
}
