using System;

namespace MOSS.Common.Code
{
    public class UploadedFile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }        
    }
}
