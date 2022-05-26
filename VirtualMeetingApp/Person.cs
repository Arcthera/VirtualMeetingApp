using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMeetingApp
{
    public class Person
    {
        public string Name { get; }
        public string Pin { get; }

        public Person(string name, string pin)
        {
            this.Name = name;
            this.Pin = pin;
        }
    }
}
