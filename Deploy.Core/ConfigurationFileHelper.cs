using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deploy.Core
{
    public class ConfigurationFileHelper
    {

        private static string rootRegistryKey = @"HKEY_CURRENT_USER\SOFTWARE\Borland\Delphi\7.0";
        private static string variablesRegistryKey = @"HKEY_CURRENT_USER\SOFTWARE\Borland\Delphi\7.0\Environment Variables";
        private static string libraryRegistryKey = @"HKEY_CURRENT_USER\SOFTWARE\Borland\Delphi\7.0\Library";

        private string _searchPath;

        private string IncludeDirectories { get { return this._searchPath; } }
        private string UnitDirectories { get { return this._searchPath; } }
        private string ResourceDirectories { get { return this._searchPath; } }

        private string _cartsysVariable;
        private string _libD7Variable;

        private string _dofFile;
        private string _fileName;
        private string _rootDir;



        public ConfigurationFileHelper(string FileName, string DofFile)
        {
            this._dofFile = DofFile;
            this._fileName = FileName;
        }

        private string ReadStringFromSection(SharpConfig.Configuration Config, string Section, string Key, string DefaultValue)
        {
            if (Config[Section] != null)
            {
                SharpConfig.Section sec = Config[Section];
                if (sec[Key] != null)
                    //return sec[Key].StringValue + (sec[Key].Comment != null ? sec[Key].Comment.Value.Value.Trim() : "");
                    return sec[Key].StringValue + sec[Key].Comment; //(sec[Key].Comment != null ? sec[Key].Comment.Value.Value.Trim() : "");
                else
                    return DefaultValue;
            }
            else
                return DefaultValue;
        }

        private void LoadCartsysVariable()
        {
            this._cartsysVariable = Registry.GetValue(variablesRegistryKey, "CARTSYS", "").ToString();
        }

        private void LoadLibD7Variable()
        {
            this._libD7Variable = Registry.GetValue(variablesRegistryKey, "LIB_D7", "").ToString();
        }

        private void LoadSearchPath()
        {
            LoadCartsysVariable();
            LoadLibD7Variable();
            LoadRootDir();
            this._searchPath = Registry.GetValue(libraryRegistryKey, "Search Path", "").ToString();
            this._searchPath = this._searchPath.Replace("$(DELPHI)", this._rootDir);
            this._searchPath = this._searchPath.Replace("$(LIB_D7)", this._libD7Variable);
            this._searchPath = this._searchPath.Replace("$(CARTSYS)", this._cartsysVariable);
            this._searchPath = this._searchPath + ";" + Path.GetDirectoryName(this._dofFile);
        }

        /*private void LoadIncludeDirectories()
        {
            this._includeDirectories = "";
        }

        private void LoadUnitDirectories()
        {
            this._unitDirectories = "";
        }

        private void LoadResourceDirectories()
        {
            this._resourceDirectories = "";
        }*/

        private void LoadRootDir()
        {
            this._rootDir = Registry.GetValue(rootRegistryKey, "RootDir", "").ToString();
        }

        public bool GenerateConfigurarionFile()
        {
            SharpConfig.Configuration config = null;
            try
            {
                config = SharpConfig.Configuration.LoadFromFile(this._dofFile);
            }
            catch
            {
                return false;
            }

            try
            {
                LoadSearchPath();//implicit load rootDir

                using (System.IO.StreamWriter fileout = new System.IO.StreamWriter(this._fileName, false))
                {
                    string FileVersion = ReadStringFromSection(config, "FileVersion", "Version", "<unknown>");

                    string s = "";
                    // -- $-Switches

                    // Record Alignment
                    s = ReadStringFromSection(config, "Compiler", "A", "8");
                    if (s == "0")
                        fileout.WriteLine("-$A-");
                    else if (s == "1")
                    {
                        if ((FileVersion == "<unknown>") | (FileVersion == "5.0"))
                            fileout.WriteLine("-$A+");
                        else
                            fileout.WriteLine("-$A" + s);
                    }
                    else
                        fileout.WriteLine("-$A" + s);

                    // Bool Eval
                    s = ReadStringFromSection(config, "Compiler", "B", "0");
                    if (s == "0")
                        fileout.WriteLine("-$B-");
                    else
                        fileout.WriteLine("-$B+");

                    // Assertions
                    s = ReadStringFromSection(config, "Compiler", "C", "1");
                    if (s == "0")
                        fileout.WriteLine("-$C-");
                    else
                        fileout.WriteLine("-$C+");

                    // DebugInfo
                    s = ReadStringFromSection(config, "Compiler", "D", "1");
                    if (s == "0")
                        fileout.WriteLine("-$D-");
                    else
                        fileout.WriteLine("-$D+");

                    // unknown, probably never used
                    s = ReadStringFromSection(config, "Compiler", "E", "0");
                    if (s == "0")
                        fileout.WriteLine("-$E-");
                    else
                        fileout.WriteLine("-$E+");

                    // unknown, probably never used
                    s = ReadStringFromSection(config, "Compiler", "F", "0");
                    if (s == "0")
                        fileout.WriteLine("-$F-");
                    else
                        fileout.WriteLine("-$F+");

                    // ImportedData
                    s = ReadStringFromSection(config, "Compiler", "G", "1");
                    if (s == "0")
                        fileout.WriteLine("-$G-");
                    else
                        fileout.WriteLine("-$G+");

                    // LongStrings
                    s = ReadStringFromSection(config, "Compiler", "H", "1");
                    if (s == "0")
                        fileout.WriteLine("-$H-");
                    else
                        fileout.WriteLine("-$H+");

                    // IOChecking
                    s = ReadStringFromSection(config, "Compiler", "I", "1");
                    if (s == "0")
                        fileout.WriteLine("-$I-");
                    else
                        fileout.WriteLine("-$I+");

                    // WritableConsts
                    s = ReadStringFromSection(config, "Compiler", "J", "0");
                    if (s == "0")
                        fileout.WriteLine("-$J-");
                    else
                        fileout.WriteLine("-$J+");

                    // Unknown, probably never used
                    s = ReadStringFromSection(config, "Compiler", "K", "0");
                    if (s == "0")
                        fileout.WriteLine("-$K-");
                    else
                        fileout.WriteLine("-$K+");

                    // LocalDebugSymbols
                    s = ReadStringFromSection(config, "Compiler", "L", "1");
                    if (s == "0")
                        fileout.WriteLine("-$L-");
                    else
                        fileout.WriteLine("-$L+");

                    // RuntimeTypeInfo
                    s = ReadStringFromSection(config, "Compiler", "M", "0");
                    if (s == "0")
                        fileout.WriteLine("-$M-");
                    else
                        fileout.WriteLine("-$M+");

                    // Unknown, probably never used
                    s = ReadStringFromSection(config, "Compiler", "N", "0");
                    if (s == "0")
                        fileout.WriteLine("-$N-");
                    else
                        fileout.WriteLine("-$N+");

                    // Optimization
                    s = ReadStringFromSection(config, "Compiler", "O", "1");
                    if (s == "0")
                        fileout.WriteLine("-$O-");
                    else
                        fileout.WriteLine("-$O+");

                    // OpenStringParams
                    s = ReadStringFromSection(config, "Compiler", "P", "1");
                    if (s == "0")
                        fileout.WriteLine("-$P-");
                    else
                        fileout.WriteLine("-$P+");

                    // IntegerOverflowChecking
                    s = ReadStringFromSection(config, "Compiler", "Q", "0");
                    if (s == "0")
                        fileout.WriteLine("-$Q-");
                    else
                        fileout.WriteLine("-$Q+");

                    // RangeChecking
                    s = ReadStringFromSection(config, "Compiler", "R", "0");
                    if (s == "0")
                        fileout.WriteLine("-$R-");
                    else
                        fileout.WriteLine("-$R+");

                    // Unknown, probably never used
                    s = ReadStringFromSection(config, "Compiler", "S", "0");
                    if (s == "0")
                        fileout.WriteLine("-$S-");
                    else
                        fileout.WriteLine("-$S+");

                    // Typed @ operator
                    s = ReadStringFromSection(config, "Compiler", "T", "0");
                    if (s == "0")
                        fileout.WriteLine("-$T-");
                    else
                        fileout.WriteLine("-$T+");

                    // PentiumSaveDivide
                    s = ReadStringFromSection(config, "Compiler", "U", "0");
                    if (s == "0")
                        fileout.WriteLine("-$U-");
                    else
                        fileout.WriteLine("-$U+");

                    // StrictVarStrings
                    s = ReadStringFromSection(config, "Compiler", "V", "1");
                    if (s == "0")
                        fileout.WriteLine("-$V-");
                    else
                        fileout.WriteLine("-$V+");

                    // GenerateStackFrames
                    s = ReadStringFromSection(config, "Compiler", "W", "0");
                    if (s == "0")
                        fileout.WriteLine("-$W-");
                    else
                        fileout.WriteLine("-$W+");

                    // ExtendedSyntax
                    s = ReadStringFromSection(config, "Compiler", "X", "1");
                    if (s == "0")
                        fileout.WriteLine("-$X-");
                    else
                        fileout.WriteLine("-$X+");

                    // SymbolReferenceInfo
                    s = ReadStringFromSection(config, "Compiler", "Y", "2");
                    if (s == "0")
                        fileout.WriteLine("-$Y-");
                    else if (s == "1")
                        fileout.WriteLine("-$YD");
                    else
                        fileout.WriteLine("-$Y+");

                    // Minimum Size of Enum
                    s = ReadStringFromSection(config, "Compiler", "Z", "1");
                    fileout.WriteLine("-$Z" + s);

                    // -- no "$" options

                    // MapFile
                    s = ReadStringFromSection(config, "Linker", "MapFile", "0");
                    if (s == "1")
                        fileout.WriteLine("-GS"); // with segements;
                    else if (s == "2")
                        fileout.WriteLine("-GP"); // with publics;
                    else if (s == "3")
                        fileout.WriteLine("-GP"); // detailed;
                    else
                        ; // no map file

                    // OutputObjs
                    s = ReadStringFromSection(config, "Linker", "OutputObjs", "0");
                    if (s == "9")
                        fileout.WriteLine("-J"); // C object files;
                    else if (s == "10")
                        fileout.WriteLine("-JP"); // C++ object files;
                    else if (s == "14")
                        fileout.WriteLine("-JPN"); // C++ object files + Namespaces;
                    else if (s == "30")
                        fileout.WriteLine("-JPNE"); // C++ object files + Namespaces + all symbols;
                    else if (s == "26")
                        fileout.WriteLine("-JPE"); // C++ object files + all symbols;
                    else
                        ; // no object files

                    // ConsoleApp (dcc help output seems to be wrong)
                    s = ReadStringFromSection(config, "Linker", "ConsoleApp", "1");
                    if (s == "1")
                        fileout.WriteLine("-cg");
                    else
                        fileout.WriteLine("-cc");

                    // remote debug info
                    s = ReadStringFromSection(config, "Linker", "RemoteSymbols", "0");
                    if (s == "1")
                        fileout.WriteLine("-vr");

                    // UnitAliases
                    s = ReadStringFromSection(config, "Compiler", "UnitAliases", "");
                    fileout.WriteLine("-A" + s);

                    // ShowHints
                    s = ReadStringFromSection(config, "Compiler", "ShowHints", "1");
                    if (s == "1")
                        fileout.WriteLine("-H+");

                    // ShowWarnings
                    s = ReadStringFromSection(config, "Compiler", "ShowWarnings", "1");
                    if (s == "1")
                        fileout.WriteLine("-W+");

                    // Make modified units - seems to be always there
                    fileout.WriteLine("-M");

                    // StackSize (also -$M)
                    s = ReadStringFromSection(config, "Linker", "MinStackSize", "16384");
                    s = s + "," + ReadStringFromSection(config, "Linker", "MaxStackSize", "1048576");
                    fileout.WriteLine("-$M" + s);

                    // ImageBase
                    s = ReadStringFromSection(config, "Linker", "ImageBase", "4194304");
                    fileout.WriteLine(string.Format("-K${0,8:X8}", Convert.ToInt32(s)));

                    // (Exe)OutputDir
                    //s = ReadStringFromSection(config, "Directories", "OutputDir", "");
                    s = Application.StartupPath;
                    fileout.WriteLine("-E\"" + s + "\"");

                    // UnitOutputDir
                    s = ReadStringFromSection(config, "Directories", "UnitOutputDir", "");
                    fileout.WriteLine("-N\"" + s + "\"");

                    // BplOutputDir
                    s = ReadStringFromSection(config, "Directories", "PackageDLLOutputDir", "");
                    fileout.WriteLine("-LE\"" + s + "\"");

                    // DcpOutputDir
                    s = ReadStringFromSection(config, "Directories", "PackageDCPOutputDir", "");
                    fileout.WriteLine("-LN\"" + s + "\"");

                    // Unit directories
                    s = ReadStringFromSection(config, "Directories", "SearchPath", "");
                    s = s.Replace("$(DELPHI)", this._rootDir);
                    fileout.WriteLine("-U\"" + s + "\"");

                    // Object directories
                    s = ReadStringFromSection(config, "Directories", "SearchPath", "");
                    s = s.Replace("$(DELPHI)", this._rootDir);
                    fileout.WriteLine("-O\"" + s + "\"");

                    // IncludeDirs
                    s = ReadStringFromSection(config, "Directories", "SearchPath", "");
                    s = s.Replace("$(DELPHI)", this._rootDir);
                    fileout.WriteLine("-I\"" + s + "\"");

                    // look for 8.3 filenames - never used
                    // fileout.WriteLine("-P");

                    // Quiet compile - never used
                    // fileout.WriteLine("-Q");

                    // Resource directories
                    s = ReadStringFromSection(config, "Directories", "SearchPath", "");
                    s = s.Replace("$(DELPHI)", this._rootDir);
                    fileout.WriteLine("-R\"" + s + "\"");

                    // Conditionals
                    s = ReadStringFromSection(config, "Directories", "Conditionals", "");
                    fileout.WriteLine("-D" + s);

                    // DebugInfo in exe
                    s = ReadStringFromSection(config, "Linker", "DebugInfo", "0");
                    if (s == "1")
                        fileout.WriteLine("-vn");

                    // Output "never build" DCPs - (not supported, cannot be read from .dof)
                    // fileout.WriteLine("-Z");

                    // -- Runtime package support
                    s = ReadStringFromSection(config, "Directories", "UsePackages", "");
                    if (s == "1")
                    {
                        s = ReadStringFromSection(config, "Directories", "Packages", "");
                        fileout.WriteLine("-LU" + s);
                    }

                    fileout.WriteLine("-U\"" + this.UnitDirectories + "\"");
                    fileout.WriteLine("-R\"" + this.ResourceDirectories + "\"");
                    fileout.WriteLine("-I\"" + this.IncludeDirectories + "\"");

                    //    cfg.WriteBOM = False;
                    //Cfg.SaveToFile(CfgFn);
                    return true;
                }
            }
            catch
            {
                return false;
            }


        }

    }
}
