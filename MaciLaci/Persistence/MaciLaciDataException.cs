using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMaciLaci.Persistence
{
    public class MaciLaciDataException : Exception
    {
        public MaciLaciDataException() { }
        public MaciLaciDataException(string message) : base(message) { }    
    }
}
