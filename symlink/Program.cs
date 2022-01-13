using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Principal;
using System.Reflection;

namespace symlink
{
    class Program
    {
        static string[] getFiles()
        {
            string[] filePaths = Array.Empty<string>();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;
                openFileDialog.Title = "Select symlink targets";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePaths = openFileDialog.FileNames;
                }
                else
                {
                    MessageBox.Show("File path(s) invalid.");
                }
            }
            return filePaths;
        }

        static void makeFileLink(string command)
        {

            foreach (string file in getFiles())
            {
                String newCommand = command;
                String linkFileName = file.Split('\\')[^1];
                if (linkFileName.Contains('.')) newCommand += '\\' + linkFileName + '\"';
                else newCommand += '\"';
                newCommand += " \"" + file + '\"';
                Process.Start("C:\\Windows\\System32\\cmd.exe", "/c" + newCommand);
            }
        }

        static string? getFolder()
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                return folderDlg.SelectedPath;
            }
            return null;
        }

        static void makeFolderLink(string command)
        {
            string? folder = getFolder();
            if(folder == null)
            {
                MessageBox.Show("Folder path invalid.");
                return;
            }
            command += '\\' + folder.Split('\\')[^1] + '\"';
            command += " \"" + folder + '\"';
            Process.Start("C:\\Windows\\System32\\cmd.exe", "/c" + command);
        }

        [STAThreadAttribute] 
        static void Main(string[] args)
        {
            if (args.Length != 1 && args.Length != 2)
            {
                Console.WriteLine("Creates a symbolic link\n\nSYMLINK [[/D] | [/H] | [/J]] Path");
                return;
            }
            string command = "mklink ";
            string arg = args[0];
            if (arg.StartsWith("/") && arg.Length == 2)
            {
                switch (arg.Substring(1).ToLower())
                {
                    case "d":
                        command += arg + " " + '\"' + args[^1];
                        makeFolderLink(command);
                        break;
                    case "h":
                        command += arg + " " + '\"' + args[^1];
                        makeFileLink(command);
                        break;
                    case "j":
                        command += arg + " " + '\"' + args[^1];
                        makeFolderLink(command);
                        break;
                    default:
                        break;
                }
                
            }
            else
            {
                command += '\"' + args[^1];
                makeFileLink(command);
            }
        }
    }
}