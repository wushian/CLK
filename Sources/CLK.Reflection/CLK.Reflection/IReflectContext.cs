using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLK.Reflection
{
    public interface IReflectContext
    {
        // Methods
        TEntity CreateEntity<TEntity>(string sectionName) where TEntity : class;

        TEntity CreateEntity<TEntity>(string sectionName, string entityName) where TEntity : class;

        IEnumerable<TEntity> CreateAllEntity<TEntity>(string sectionName) where TEntity : class;
    }
}
