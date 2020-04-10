using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexMethod.Models
{

    public interface IMethod
    {
        string StartValue { get; set; }

        bool Validate { get; set; }

        object GetResult();

        
    }
}
