using LogonTimes.DataModel;
using System;

namespace LogonTimes
{
    public class PersonLoadedEventArgs : EventArgs
    {
        public Person PersonLoaded { get; set; }

        public PersonLoadedEventArgs(Person person)
        {
            PersonLoaded = person;
        }
    }
}
