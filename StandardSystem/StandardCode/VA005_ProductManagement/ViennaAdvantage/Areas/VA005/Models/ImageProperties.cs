using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VA005.Models
{
    public class ImageProperties
    {
        public ImageDetail ImageDetail { get; set; }
        public bool isUpload { get; set; }
    }
    public class ImageDetail
    {
        public string fileName { get; set; }
        public string fileExtension { get; set; }
        public byte fileSize { get; set; }
        public string fileBase64Data { get; set; }
        public string filePath { get; set; }
    }
   
}