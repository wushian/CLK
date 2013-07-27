using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Collections;

namespace CLK.Reflection
{
    public sealed class ReflectSection : StoreDictionary<string, ReflectBuilder>
    {
        // Constructors
        internal ReflectSection(string sectionName, IStoreProvider<string, ReflectBuilder> builderProvider) : base(builderProvider) 
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // SectionName
            this.SectionName = sectionName;
        }


        // Properties
        public string SectionName { get; private set; }
    }
}
