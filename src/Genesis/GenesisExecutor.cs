using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Genesis.Cli;

namespace Genesis
{
    public abstract class GenesisExecutor<TResultType> : IGenesisExecutor<ITaskResult> where TResultType : ITaskResult, new()
    {
        public virtual string CommandText => throw new NotImplementedException();

        public virtual string Description => throw new NotImplementedException();

        public virtual string FriendlyName => throw new NotImplementedException();

        private bool _init;

        public virtual bool Initialized
        {
            get { return _init; }
            protected set { _init = value; }
        }

        public Task Initialize()
        {
            Initialized = false;

            OnInitilized();

            Initialized = true;

            return Task.CompletedTask;
        }

        protected virtual void OnInitilized()//TODO: Pass args[] to Generator.OnInitialized
        {

        }

        public async virtual Task<bool> EditConfig<TPropertyType>(string propertyName, TPropertyType value)
        {
            bool err = false;
            Type cfgType = null;
            try
            {
                var cfg = GetType().GetProperty("Config").GetValue(this);
                cfgType = cfg.GetType();
                cfgType.GetProperty(propertyName).SetValue(cfg, value);

                Text.Green(cfgType.Name); Text.White("."); Text.Cyan(propertyName);Text.WhiteLine(" was updated.");
            }
            catch (NullReferenceException)
            {
                Text.Red($"No property called '");Text.Cyan(propertyName);Text.RedLine($"' on {cfgType.Name}");
                err = true;
            }
            return await Task.FromResult(!err);
        }

        public abstract Task<ITaskResult> Execute(GenesisContext genesis, string args);
    }
}
