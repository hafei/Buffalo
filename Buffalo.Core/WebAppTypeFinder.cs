using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Buffalo.Core
{
    public class WebAppTypeFinder : AppDomainTypeFinder
    {
        private bool _ensureBinFolderAssemblierLoaded = true;
        private bool _binFolderAssembliesLoaded;


        public virtual string GetBinDirectory()
        {
            return System.AppContext.BaseDirectory;
        }

        public override IList<Assembly> GetAssemblies()
        {
            if (this._ensureBinFolderAssemblierLoaded && !_binFolderAssembliesLoaded)
            {
                _binFolderAssembliesLoaded = true;
                string binPath = GetBinDirectory();
                LoadMatchingAssemblies(binPath);
            }

            return base.GetAssemblies();
        }
    }
}
