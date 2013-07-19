using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Reflection
{
    public abstract class ReflectSection
    {
        // Properties
        public string SectionName { get; set; }


        // Methods
        internal abstract TEntity CreateEntity<TEntity>() where TEntity : class;

        internal abstract TEntity CreateEntity<TEntity>(string entityName) where TEntity : class;

        internal abstract IEnumerable<TEntity> CreateAllEntity<TEntity>() where TEntity : class;
    }
}
