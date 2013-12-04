using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLK.Collections;

namespace CLK.Reflection
{
    internal interface IReflectBuilderRepository : IStoreProvider<string, ReflectBuilder>
    {

    }

    internal sealed class ReflectBuilderRepository : IReflectBuilderRepository
    {
        // Fields        
        private readonly IReflectRepository _repository = null;

        private readonly string _groupName = null;        


        // Constructors
        public ReflectBuilderRepository(IReflectRepository repository, string groupName)
        {
            #region Contracts
                        
            if (repository == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(groupName) == true) throw new ArgumentNullException();

            #endregion

            // Arguments            
            _repository = repository;
            _groupName = groupName;
        }


        // Methods        
        public void Add(string entityName, ReflectBuilder builder)
        {
            #region Contracts

            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();
            if (builder == null) throw new ArgumentNullException();

            #endregion
            
            // Repository
            _repository.AddSetting(_groupName, entityName, this.ToSetting(builder));
        }

        public void Remove(string entityName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();

            #endregion

            // Repository
            _repository.RemoveSetting(_groupName, entityName);
        }

        public bool ContainsKey(string entityName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();

            #endregion

            // Repository
            return _repository.ContainsSetting(_groupName, entityName);
        }
                
        public IEnumerable<string> GetAllKey()
        {
            // Repository
            return _repository.GetAllEntityName(_groupName);
        }

        public ReflectBuilder GetValue(string entityName)
        {
            #region Contracts

            if (string.IsNullOrEmpty(entityName) == true) throw new ArgumentNullException();

            #endregion

            // Repository
            ReflectSetting setting = _repository.GetSetting(_groupName, entityName);
            if (setting == null) return null;

            // Return
            return this.ToBuilder(setting);
        }


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
    }
}
