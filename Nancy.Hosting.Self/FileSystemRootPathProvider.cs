namespace Nancy.Hosting.Self
{
    using System;
    using System.IO;
    using System.Reflection;

    public class FileSystemRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return _ReConfig.path_root;

            //var assembly = Assembly.GetEntryAssembly();

            //return assembly != null ? 
            //    Path.GetDirectoryName(assembly.Location) :
            //    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
