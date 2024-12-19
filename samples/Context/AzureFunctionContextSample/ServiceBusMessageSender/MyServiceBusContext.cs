using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ServiceBusMessageSender
{
    public class MyServiceBusContext
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Contact? MyContact { get; set; }
    }

    public class Contact
    {
        public string Email { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
    }
}
