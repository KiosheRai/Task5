using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task5.Models
{
    public class TextMessage
    {
        public int id { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }
    }
}
