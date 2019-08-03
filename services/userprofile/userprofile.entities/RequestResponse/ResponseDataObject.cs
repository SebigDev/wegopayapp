using System;
using System.Collections.Generic;
using System.Text;

namespace userprofile.entities.RequestResponse
{
   public  class ResponseDataObject<T> where T: class
    {
        public ResponseDataObject()
        {
            Status = false;
        }

        public string Message { get; set; }
        public T Data { get; set; }

        public bool Status { get; set; }
    }
}
