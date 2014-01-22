using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Project {
    using Microsoft.Build.BuildEngine;

    class BuildItemGroupShim : IEnumerable<BuildItemShim> {
        private BuildItemGroup _group;

        public BuildItemGroupShim(BuildItemGroup group) {
            _group = group;
        }

        public bool IsImported { get { return _group.IsImported; } }

        #region IEnumerable<BuildItemShim> Members

        public IEnumerator<BuildItemShim> GetEnumerator() {
            foreach (var x in _group) {
                yield return new BuildItemShim((BuildItem)x);
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            foreach (var x in _group) {
                yield return new BuildItemShim((BuildItem)x);
            }
        }

        #endregion
    }
}
