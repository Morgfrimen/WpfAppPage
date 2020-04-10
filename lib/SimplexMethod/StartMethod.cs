using System;
using System.Collections.Generic;
using System.Text;
using SimplexMethod.Models;

namespace Methods
{
    public class StartMethod
    {
        static List<IMethod> methods = new List<IMethod>()
        {
            new SimplexTable(),
        };
    }
}
