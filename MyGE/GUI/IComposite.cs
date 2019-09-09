using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    public interface IComposite
    {
        void Add(Widget pElement);
        void Remove(Widget pElement);
        Widget GetChild(int pIndex);
    }
}
