using LogonTimes.DataModel;
using System;

namespace LogonTimes.DataModel
{
    public class PersonEventArgs : EventArgs
    {
        public Person Person { get; set; }

        public PersonEventArgs(Person person)
        {
            Person = person;
        }
    }
}
