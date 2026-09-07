using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace VA005.Models
{
   // [DataContract]
    public class AttachedFileInfo
    {
      //  [DataMember]
        public string FileName { get; set; }
     //   [DataMember]
        public string FileUID { get; set; }
      //  [DataMember]
        public long Size { get; set; }
     //   [DataMember]
        public string StoredFileName { get; set; }
        //[DataMember]
        //public byte[] Data { get; set; }
       // [DataMember]
        public string FileExtension { get; set; }
       // [DataMember]
        public string FileNameWithoutExtension { get; set; }
      //  [DataMember]
        public byte[] FileBytes { get; set; }
      //  [DataMember]
        public string DocName { get; set; }
      //  [DataMember]
        public string DocComment { get; set; }
      //  [DataMember]
        public string Keyword { get; set; }
        //[DataMember]
        //public string Version { get; set; }
      //  [DataMember]
        public string MetaComment { get; set; }
      //  [DataMember]
        public DateTime? FileCreatedDate { get; set; }
      //  [DataMember]
        public string Description { get; set; }
      //  [DataMember]
        public string EmailUID { get; set; }
      //  [DataMember]
        public int StartIndex { get; set; }
       // [DataMember]
        public int EndIndex { get; set; }
      //  [DataMember]
        public int Length { get; set; }
        public DateTime latestModifiedDate { get; set; }
        public string folderKey { get; set; }
        public double TotalNoOfFileSize { get; set; }
    }
}
