namespace SPCAFContrib.Test.SearchString
{
    public class TestStringClassImpl
    {
        #region contructors

        public TestStringClassImpl()
        {
            PropertyString = "PropertyString.Test";
            SetProperty("SetProperty.Test");
        }

        #endregion

        public string FieldString = "FieldString.Test";
        public string PropertyString { get; set; }

        public string PropertyGetString
        {
            get
            {
                return @"PropertyGetString.Test";
            }
        }

        public static string StaticPropertyString = @"StaticPropertyString.Test";
        public const string ConstFieldString = @"ConstFieldString.Test";

        #region methods

        public void SetProperty(string value)
        {

        }

        #endregion
    }
}
