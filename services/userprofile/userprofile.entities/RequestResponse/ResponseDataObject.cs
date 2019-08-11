using System;
using System.Collections.Generic;
using System.Text;

namespace userprofile.entities.RequestResponse
{
   public  class ResponseDataObject 
    {
        public string Message { get; set; }
  
        public bool Status { get; set; }
    }


    public class ResponseDataObject<T> : ResponseDataObject 
    {
       public  T Data { get; set; }

    }

    public class ResponseDataObjectList<T> : ResponseDataObject
    {
        public List<T> DataList { get; set; }
    }

}
