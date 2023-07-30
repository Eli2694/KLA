using Entity.EntityInterfaces;

namespace Entity
{
    public class RealFileSystem : IFileSystem
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string GetFileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }
    }
}
