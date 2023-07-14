using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Single
{
    public interface IReportSinglePattern
    {
        public IEnumerable<int> SinglePatternMatching(string pattern);
    }
}
