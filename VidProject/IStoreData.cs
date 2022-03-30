using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VidProject
{
    public interface IStoreData
    {
        void WriteFile(string[] text, bool appendOrCreate);
    }
}
