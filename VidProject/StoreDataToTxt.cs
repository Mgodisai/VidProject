using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VidProject
{
    public class StoreDataToTxt : IStoreData
    {
        private string filename;

        public StoreDataToTxt(string filename)
        {
            this.filename = filename;
        }

        public void WriteFile(string[] text, bool appendOrCreate)
        {
            using (StreamWriter sw = new StreamWriter(this.filename, appendOrCreate))
            {
                text.ToList().ForEach(x => sw.WriteLine(x));
            }
        }
    }
}
