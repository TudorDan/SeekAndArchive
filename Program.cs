using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SeekAndArchive
{
    class Program
    {
        static void Main(string[] args)
        {
            // Specify the directory and file you want to manipulate.
            string directoryTarget = args[1];
            string fileToSearch = args[0];

            SearchForFile(directoryTarget, fileToSearch);
        }

        public static void SearchForFile(string directory, string fileName)
        {
            // Take a snapshot of the file system.  
            DirectoryInfo dir = new DirectoryInfo(directory);

            if (dir.Exists)
            {
                // This method assumes that the application has discovery permissions  
                // for all folders under the specified path.  
                IEnumerable<FileInfo> fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);

                //Create the query  
                IEnumerable<FileInfo> fileQuery =
                    from file in fileList
                    where file.Name == fileName
                    select file;

                //Execute the query. This might write out a lot of files!  
                foreach (FileInfo fi in fileQuery)
                {
                    Console.WriteLine(fi.FullName);
                }
                if (!fileQuery.Any())
                {
                    Console.WriteLine($"No files named '{fileName}' were found in directory '{directory}'.");
                }
            }
            else
            {
                Console.WriteLine($"Directory '{directory}' was not found!");
            }
        }
    }
}
