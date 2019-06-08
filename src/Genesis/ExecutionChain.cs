using Genesis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Genesis
{
    public class ExecutionChain 
    {
        protected GenesisContext _ctx;
        protected LinkedList<IGenesisExecutor<ITaskResult>> ll;

        public int Count { get => ll.Count; }

        public ExecutionChain(GenesisContext genesisContext)
        {
            _ctx = genesisContext;
            ll = new LinkedList<IGenesisExecutor<ITaskResult>>();
        }

        public Task Append(IGenesisExecutor<ITaskResult> executor)
        {
            ll.AddLast(executor);

            return Task.CompletedTask;
        }

        public Task DisplayDetail()
            => ForEach(e => {
                    Text.Command(e.CommandText);
                    Text.White(" from ");
                    Text.Assembly(e.GetType().Assembly.GetName().Name);
                    Text.WhiteLine(".");
                });        

        protected Task ForEach(Action<IGenesisExecutor<ITaskResult>> action)
        {
            var node = ll.First;
            while (node != null && node.Value != null)
            {
                action(node.Value);
                node = node.Next;
            }

            return Task.CompletedTask;
        }

        public List<ITaskResult> Execute(string[] args)
        {
            Text.WhiteLine($"Beginning serial execution of {ll.Count} executors.");

            var results = new List<ITaskResult>();

            ForEach(e => {
                ITaskResult result = null;
                try
                {
                    Text.Execute($"{e.GetType().Name}");
                    result = e.Execute(_ctx, args).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Text.RedLine(ex.Message);
                }
                finally
                {
                    results.Add(result);
                }
            });

            return results;
        }

        public void Clear() 
            => ll.Clear();
    }
}
