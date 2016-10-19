using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deploy.Core
{
    public class CompileHelper
    {

        private static string rootRegistryKey = @"HKEY_CURRENT_USER\SOFTWARE\Borland\Delphi\7.0";

        private string _compilerPath;

        private string _delphiProjectFileName;

        private void LoadRootDir()
        {
            this._compilerPath = Registry.GetValue(rootRegistryKey, "RootDir", "").ToString() + "\\Bin\\DCC32.EXE";
        }

        public CompileHelper(string DelphiProjectFileName)
        {
            this._delphiProjectFileName = DelphiProjectFileName;
            LoadRootDir();
        }

        public string CompileProject()
        {

            Process p = new Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(this._delphiProjectFileName);
            p.StartInfo.FileName = this._compilerPath;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.StartInfo.Arguments = "\"" + this._delphiProjectFileName + "\"";
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            if (output.Contains("Fatal:"))
                return output;
            else
                return "";
        }
    }
}
