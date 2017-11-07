using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Builder
{
    public interface IParser
    {
        T1 Parse<T1>(string input);

        string Stringify<T2>(T2 obj);
    }
}
