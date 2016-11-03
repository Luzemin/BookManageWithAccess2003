using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Book.Models
{
    public class BookModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Press { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public DateTime CreateTime { get; set; }
    }
}