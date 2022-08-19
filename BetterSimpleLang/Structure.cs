using System;
using System.Collections.Generic;
using System.Text;

namespace BetterSimpleLang
{
    public class StructureField
    {
        public string Name;
        public Type Type;
        public string TypeName;
        public bool IsConstant;
        public object Value;
    }

    // TODO: reference variables in structs
    public class Structure
    {
        public string Name;
        public StructureField[] Fields;
    }
}
