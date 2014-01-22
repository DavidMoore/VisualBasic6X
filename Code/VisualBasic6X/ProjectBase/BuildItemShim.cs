using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Project {
    using Microsoft.Build.BuildEngine;

    class BuildItemShim {
        private BuildItem _buildItem;

        public BuildItemShim(BuildItem buildItem) {
            _buildItem = buildItem;
        }
        public string FinalItemSpec { get { return _buildItem.FinalItemSpec; } }
    
        public  string Name { get { return _buildItem.Name; } }
    }
}
