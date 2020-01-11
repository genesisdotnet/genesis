using Genesis.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Genesis
{
    public class ExecutionChain 
    {
        protected GenesisContext _ctx;
        protected LinkedList<IGenesisExecutor<IGenesisExecutionResult>> ll;

        public int Count { get => ll.Count; }

        public ExecutionChain(GenesisContext genesisContext)
        {
            _ctx = genesisContext;
            ll = new LinkedList<IGenesisExecutor<IGenesisExecutionResult>>();
        }

        public Task Append(IGenesisExecutor<IGenesisExecutionResult> executor)
        {
            ll.AddLast(executor);

            return Task.CompletedTask;
        }

        public Task DisplayDetail()
            => ForEach(e => {
                    Text.CliCommand(e.CommandText);
                    Text.White(" from ");
                    Text.Assembly(e.GetType().Assembly.GetName().Name);
                    Text.GrayLine(".");
                });        

        public Task ForEach(Action<IGenesisExecutor<IGenesisExecutionResult>> action)
        {
            var node = ll.First;
            while (node != null && node.Value != null)
            {
                action(node.Value);
                node = node.Next;
            }

            return Task.CompletedTask;
        }

        public List<IGenesisExecutionResult> Execute(string[] args)
        {
            Text.Line();
            Text.WhiteLine($"Beginning serial execution of {ll.Count} executors.");

            var results = new List<IGenesisExecutionResult>();

            ForEach(e => {
                IGenesisExecutionResult result = null;
                try
                {
                    Text.Execute($"{e.GetType().Name}"); Text.Line();
                    result = e.Execute(_ctx, args).GetAwaiter().GetResult(); // no SyncContext hopefully
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
