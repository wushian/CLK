using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Collections;

namespace CLK.Reflection
{
    public sealed class ReflectSectionDictionary : StoreDictionary<string, ReflectSection>
    {
        // Constructors
        internal ReflectSectionDictionary(IStoreProvider<string, ReflectSection> provider) : base(provider) { }
    }
}
