namespace Entity.EntityInterfaces
{
    public interface IFileSystem
    {
        bool FileExists(string path);
        string GetFileExtension(string path);
        void WriteAllText(string path, string contents);
    }
}
