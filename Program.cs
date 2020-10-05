using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;

namespace SeekAndArchive
{
    class Program
    {
        static void Main()
        {
            // Specify the directory and file you want to manipulate.
            string[] args = Environment.GetCommandLineArgs();
            string directoryTarget = args[2];
            string fileToSearch = args[1];

            // If a directory is not specified, exit program.
            if (args.Length != 3)
            {
                // Display the proper way to call the program.
                Console.WriteLine("Usage: SeekAndArchive.exe file directory");
                return;
            }

            IEnumerable<FileInfo> fileList = SearchForFile(directoryTarget, fileToSearch);

            WatchForFile(fileList, fileToSearch);
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private static void WatchForFile(IEnumerable<FileInfo> paths, string file)
        {
            List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

            foreach (FileInfo fi in paths)
            {
                // Create a new FileSystemWatcher and set its properties.
                FileSystemWatcher watcher = new FileSystemWatcher();

                watcher.Path = fi.Directory.FullName;

                // Watch for changes in LastAccess and LastWrite times, and
                // the renaming of files or directories.
                watcher.NotifyFilter = NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName;

                // Only watch text files.
                watcher.Filter = file;

                // Add event handlers.
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;

                // Begin watching.
                watcher.EnableRaisingEvents = true;

                watchers.Add(watcher);
            }
            // Wait for the user to quit the program.
            Console.WriteLine("Press 'q' to quit the sample.");
            while (Console.Read() != 'q') ;
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }

        public static IEnumerable<FileInfo> SearchForFile(string directory, string fileName)
        {
            // Take a snapshot of the file system.  
            DirectoryInfo dir = new DirectoryInfo(directory);

            if (dir.Exists)
            {
                // This method assumes that the application has discovery permissions  
                // for all folders under the specified path.  
                IEnumerable<FileInfo> fileList = dir.GetFiles(fileName, SearchOption.AllDirectories);

                ////Create the query  
                //IEnumerable<FileInfo> fileQuery =
                //    from file in fileList
                //    where file.Name == fileName
                //    select file;

                //Execute the query. This might write out a lot of files!  
                //foreach (FileInfo fi in fileQuery)
                foreach (FileInfo fi in fileList)
                {
                    Console.WriteLine(fi.FullName);
                }
                //if (!fileQuery.Any())
                if (!fileList.Any())
                {
                    Console.WriteLine($"No files named '{fileName}' were found in directory '{directory}'.");
                }
                return fileList;
            }
            else
            {
                Console.WriteLine($"Directory '{directory}' was not found!");
            }
            return null;
        }
    }
}
