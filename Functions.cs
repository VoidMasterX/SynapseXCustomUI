using sxlib.Specialized;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse_X
{
    class Functions
    {
        public static SxLibWinForms Lib;

        public static OpenFileDialog OpenFile = new OpenFileDialog
        {
            Filter = "Script Files (*.lua, *.txt)|*.lua;*.txt",
            FilterIndex = 1,
            RestoreDirectory = true,
            Title = "Synapse X - Open File"
        };

        public static OpenFileDialog ExecuteFile = new OpenFileDialog
        {
            Filter = "Script Files (*.lua, *.txt)|*.lua;*.txt",
            FilterIndex = 1,
            RestoreDirectory = true,
            Title = "Synapse X - Execute File"
        };

        public static readonly Random Rnd = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Rnd.Next(s.Length)]).ToArray());
        }

        public static void PopulateListBox(ListBox lsb, string Folder, string FileType)
        {
            DirectoryInfo dinfo = new DirectoryInfo(Folder);
            FileInfo[] Files = dinfo.GetFiles(FileType);
            foreach (FileInfo file in Files)
            {
                lsb.Items.Add(file.Name);
            }
        }
    }
}
