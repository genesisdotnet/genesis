using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Genesis;
using Genesis.Generation;

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
        public async virtual Task DisplayConfiguration()
        {
            var cfgObj = ConfigObject();
            var cfgObjType = cfgObj.GetType();

            Text.WhiteLine();
            Text.White($"Properties for '"); Text.Cyan($"{cfgObjType.Name}"); Text.WhiteLine("'");

            foreach (var p in cfgObjType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var val = cfgObjType.GetProperty(p.Name)?.GetValue(cfgObj)?.ToString();

                Text.Yellow($"\t{p.Name}: ");
                Text.DarkBlue(p.PropertyType.Name);
                Text.White("   (");
                Text.DarkYellow($"{((val is string) ? $"\"{val}\"":val)}");
                Text.WhiteLine(")");
            }

            await Task.CompletedTask;
        }

        public async virtual Task<bool> EditConfig<TPropertyType>(string propertyName, TPropertyType value)
        {
            bool err = false;
            Type cfgType = null;
            try
            {
                var cfg = ConfigObject();
                cfgType = cfg.GetType();

                if (value.GetType().IsAssignableFrom(typeof(string)))
                {
                    var bsalloc = value.ToString().TrimStart('"').TrimEnd('"');
                    cfgType.GetProperty(propertyName).SetValue(cfg, bsalloc);
                }
                else //feels ghetto to remove quotes like this... 
                {
                    cfgType.GetProperty(propertyName).SetValue(cfg, value);
                }

                Text.Green(cfgType.Name); Text.White("."); Text.Cyan(propertyName); Text.WhiteLine(" was updated.");
            }
            catch (NullReferenceException)
            {
                Text.Red($"No property called '");Text.Cyan(propertyName);Text.RedLine($"' on {cfgType.Name}");
                err = true;
            }
            return await Task.FromResult(!err);
        }


        protected object ConfigObject() 
            => GetType().GetProperty("Config").GetValue(this);

        public abstract Task<ITaskResult> Execute(GenesisContext genesis, string[] args);
    }
}
