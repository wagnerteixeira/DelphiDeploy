using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deploy.Core
{
    public class Service
    {
        private string _delphiProjectFileName;
        private string _dcuPath;

        public Service(string DelphiProjectFileName, string DcuPath)
        {
            this._delphiProjectFileName = DelphiProjectFileName;
            this._dcuPath = DcuPath;
        }

        public string CompileProject()
        {
            string configurationFile = Path.GetDirectoryName(this._delphiProjectFileName) + "\\" + Path.GetFileNameWithoutExtension(this._delphiProjectFileName) + ".cfg";
            string dofFile = Path.GetDirectoryName(this._delphiProjectFileName) + "\\" + Path.GetFileNameWithoutExtension(this._delphiProjectFileName) + ".dof";
            if (!File.Exists(dofFile))
            {
                return "Arquivo " + dofFile + " não encontrado";
            }
            ConfigurationFileHelper cfgHelper = new ConfigurationFileHelper(configurationFile, dofFile);
            cfgHelper.GenerateConfigurarionFile();
            foreach (string file in Directory.GetFiles(_dcuPath, "*.dcu", SearchOption.TopDirectoryOnly))
            {
                File.Delete(file);
            }
            CompileHelper compile = new CompileHelper(this._delphiProjectFileName);
            return compile.CompileProject();
        }
    }
}
