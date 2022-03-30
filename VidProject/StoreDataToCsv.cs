using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VidProject
{
    public class StoreDataToCsv : IStoreData
    {
        private string filename;

        public StoreDataToCsv(string filename)
        {
            this.filename = filename;
        }

        public void WriteFile(string[] text, bool appendOrCreate)
        {
            using (StreamWriter sw = new StreamWriter(this.filename, appendOrCreate))
            {
                sw.WriteLine(text[0] + ";" + text[1]);
            }
        }
    }
}
