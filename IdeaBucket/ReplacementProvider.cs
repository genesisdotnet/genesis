using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Genesis.Suggestions
{
    /*
    public interface IGenesisExecutor<TGenesisExecutionResult> where TGenesisExecutionResult : IGenesisExecutionResult
    {
        string CommandText { get; }
        string Description { get; }
        string FriendlyName { get; }

        bool Initialized { get; }
        Task Initialize();
        Task DisplayConfiguration();
        Task<bool> EditConfig<TPropertyType>(string propertyName, TPropertyType value);
        Task<TGenesisExecutionResult> Execute(GenesisContext genesis, string[] args);
    }
    */

    /*
    Definition =>
        Entities
            Person
                Name
                Id
            User
                UserName
                Email
            Contact
                Address1
                Address2

    Model =>
        Object
            Person
                Properties

    Description =>
        Object 
            Keys
                Objectname
                Propert1
                Propt[]

    Exection
        I!MyType!
        !MtYpe



     * 
     Scoping
        key, value
        key, value
     */

    public interface IReplacementPaser
    {
        IEnumerable<(string key, Func<object, string> execFunc)> Parse(string template);
    }

    public abstract class ReplacementPaserBase<T> : IReplacementPaser
    {
        public abstract IEnumerable<(string key, Func<T, string> execFunc)> Parse(string template);

        IEnumerable<(string key, Func<object, string> execFunc)> IReplacementPaser.Parse(string template)
        {
            return from e in Parse(template)
                   let k = e.key
                   let f = new Func<object?, string?>(o => o is T t ? e.execFunc(t) : null)
                   select (k, f);
        }
    }
    public class ReplacementProvider<T>
    {
        private readonly IEnumerable<(string key, Func<T, string> execFunc)> _replacements;

        public ReplacementProvider(IEnumerable<KeyValuePair<string, Func<T, string>>> replacements)
            : this(replacements.Select(kvp => (kvp.Key, kvp.Value)))
        {
        }
        public ReplacementProvider(IEnumerable<(string key, Func<T, string> execFunc)> replacements)
        {
            _replacements = replacements;
        }
        public ReplacementProvider(params (string key, Func<T, string> execFunc)[] replacements)
            : this(replacements.AsEnumerable())
        {
        }

        public string DoShit(T input, string template)
        {
            var sb = new StringBuilder(template);

            foreach (var r in _replacements)
            {
                sb.Replace(r.key, r.execFunc(input));
            }
            return sb.ToString();
        }
    }

    public class ReplacementProviderFactory
    {
        public ReplacementProvider<T> Get<T>()
        {
            throw new NotImplementedException();
        }
    }
}
