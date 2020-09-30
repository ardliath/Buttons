using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buttons.Data
{
    public class Dependency : IDependency
    {
        public string GetText()
        {
            return "Injected ";
        }
    }

    public interface IDependency
    {
        string GetText();
    }
}
