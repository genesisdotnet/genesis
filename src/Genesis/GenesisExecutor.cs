using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Genesis;
using Genesis.Output;

namespace Genesis
{
    public abstract class GenesisExecutor<GenesisExecutionResult> : IGenesisExecutor<IGenesisExecutionResult>
    {
        public virtual string CommandText => throw new NotImplementedException();

        public virtual string Description => throw new NotImplementedException();

        public virtual string FriendlyName => throw new NotImplementedException();

        public virtual bool Initialized { get; protected set; }

        public Task Initialize()
        {
            Initialized = false;

            OnInitialized();

            //TODO: Load Key / Action pairs from .genesis script

            Initialized = true;

            return Task.CompletedTask;
        }

        protected virtual void OnInitialized()//TODO: Pass args[] to OutputExecutor.OnInitialized
        {

        }

        public async virtual Task DisplayConfiguration()
        {
            var cfgObj = ConfigObject();
            var cfgObjType = cfgObj.GetType();

            Text.WhiteLine();
            Text.CliCommand(CommandText);
            Text.White(" configuration type is: "); Text.CyanLine($"{cfgObjType.Name}");

            foreach (var p in cfgObjType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var val = cfgObjType.GetProperty(p.Name)?.GetValue(cfgObj);

                Text.Yellow($"\t{p.Name}: ");
                Text.DarkBlue(p.PropertyType.Name);
                Text.White(":");
                Text.DarkYellowLine($"{(val != null ? val is string ? $"\"{val}\"" : val : $"")}");
            }

            await Task.CompletedTask;
        }

        public async virtual Task<bool> EditConfig<TPropertyType>(string propertyName, TPropertyType value)
        {
            var err = false;
            Type cfgType = null;
            try
            {
                var cfg = ConfigObject();
                cfgType = cfg.GetType();

                if (value.GetType().IsAssignableFrom(typeof(string)))
                {
                    var bsalloc = value.ToString().TrimStart('"').TrimEnd('"');
                    cfgType.GetProperty(propertyName)?.SetValue(cfg, bsalloc);
                }
                else //feels ghetto to remove quotes like this... 
                {
                    cfgType.GetProperty(propertyName)?.SetValue(cfg, value);
                }

                //Text.Green(cfgType.Name); Text.White("."); Text.Cyan(propertyName); Text.WhiteLine(" was updated.");
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

        public abstract Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args);
    }
}
