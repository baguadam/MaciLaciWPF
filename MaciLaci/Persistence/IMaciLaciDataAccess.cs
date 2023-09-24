using MaciLaci.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMaciLaci.Persistence
{
    public interface IMaciLaciDataAccess
    {
        public Table Load(String path);

        public void Save(String path, Table table);
    }
}
