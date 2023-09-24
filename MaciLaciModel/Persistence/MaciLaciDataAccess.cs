using MaciLaci.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMaciLaci.Persistence
{
    public class MaciLaciDataAccess : IMaciLaciDataAccess
    {
        public Table Load(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String line = reader.ReadLine() ?? string.Empty;
                    String[] number = line.Split(' ');
                    Int32 tableSize = Int32.Parse(number[0]);

                    Table table = new Table(tableSize);

                    for (int i = 0; i < tableSize; i++)
                    {
                        line = reader.ReadLine() ?? string.Empty;
                        number = line.Split(' ');

                        for (int j = 0; j < tableSize; j++)
                        {
                            table.SetValue(i, j, (FieldOptions)Enum.Parse(typeof(FieldOptions), number[j]));
                        }
                    }
                    return table;
                }
            } catch
            {
                throw new MaciLaciDataException();
            }
        }

        public void Save(String path, Table table)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(table.Size + " ");
                    writer.WriteLine();
                    for (int i = 0; i < table.Size; i++)
                    {
                        for (int j = 0; j < table.Size; j++)
                        {
                            writer.Write((int)table.GetValue(i, j) + " ");
                        }
                        writer.WriteLine();
                    }
                }
            }
            catch
            {
                throw new MaciLaciDataException();
            }

        }
    }
}
