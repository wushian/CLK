using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Collections;

namespace CLK.Reflection
{
    internal sealed class ReflectBuilderRepository : IStoreProvider<string, ReflectBuilder>
    {
        // Fields        
        private readonly string _sectionName = null;

        private readonly IReflectRepository _repository = null;


        // Constructors
        public ReflectBuilderRepository(string sectionName, IReflectRepository repository)
        {
            #region Contracts

            if (string.IsNullOrEmpty(sectionName) == true) throw new ArgumentNullException();

            #endregion

            // Arguments
            _sectionName = sectionName;
            _repository = repository;
        }


        // Methods
        private ReflectBuilder ToBuilder(ReflectSetting setting)
        {
            #region Contracts

            if (setting == null) throw new ArgumentNullException();

            #endregion            

            // Type
            Type type = Type.GetType(setting.BuilderType);
            if (type == null) throw new InvalidOperationException(string.Format("Fail to create type:{0}", setting.BuilderType));

            // Builder
            ReflectBuilder builder = Activator.CreateInstance(type) as ReflectBuilder;
            if (type == null) throw new InvalidOperationException(string.Format("Fail to create instance:{0}", setting.BuilderType));

            // Parameters
            foreach (string parameterKey in setting.Parameters.Keys)
            {
                builder.Parameters.Add(parameterKey, setting.Parameters[parameterKey]);
            }

            // Return
            return builder;
        }

        private ReflectSetting ToSetting(ReflectBuilder builder)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException();

            #endregion

            // Create
            ReflectSetting setting = new ReflectSetting(builder.GetType().AssemblyQualifiedName);
            foreach (string parameterKey in builder.Parameters.Keys)
            {
                setting.Parameters.Add(parameterKey, builder.Parameters[parameterKey]);
            }

            // Return
            return setting;
        }


        public void Add(string entityName, ReflectBuilder builder)
        {
            #region Contracts

            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();
            if (builder == null) throw new ArgumentNullException();

            #endregion
            
            // Repository
            _repository.AddSetting(_sectionName, entityName, this.ToSetting(builder));
        }

        public void Remove(string entityName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();

            #endregion

            // Repository
            _repository.RemoveSetting(_sectionName, entityName);
        }

        public bool ContainsKey(string entityName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();

            #endregion

            // Repository
            return _repository.ContainsSetting(_sectionName, entityName);
        }

        public ReflectBuilder GetValue(string entityName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();

            #endregion

            // Repository
            ReflectSetting setting = _repository.GetSetting(_sectionName, entityName);
            if (setting == null) return null;

            // Return
            return this.ToBuilder(setting);
        }

        public IEnumerable<string> GetAllKey()
        {
            // Repository
            return _repository.GetAllEntityName(_sectionName);
        }
    }
}
