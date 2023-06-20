using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.EntityInterfaces
{
    public interface IFileSystem
    {
        bool FileExists(string path);
        string GetFileExtension(string path);
        void WriteAllText(string path, string contents);
    }
}
