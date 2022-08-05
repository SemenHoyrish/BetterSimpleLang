using System;
using System.Collections.Generic;
using System.Text;

namespace BetterSimpleLang
{
    public class StructureField
    {
        public string Name;
        public IType Type;
    }

    // TODO: reference variables in structs
    public class Structure
    {
        public string Name;
        public StructureField[] Fields;
    }
}
