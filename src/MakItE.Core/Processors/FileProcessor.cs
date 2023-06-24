using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace MakItE.Core.Processors
{
    public class FileProcessor<TData, TResult>
        where TData : class
        where TResult : class
    {
        const int DefaultDataChannelCapacity = 20;
        const int DefaultMessageChannelCapacity = 100;

        readonly ConcurrentBag<TResult> resultCollection;

        private Channel<TData>? dataChannel;
        private Channel<Diagnostic>? messageChannel;

        private int workerCount = -1;

        readonly BoundedChannelOptions dataChannelOptions;
        readonly BoundedChannelOptions messageChannelOptions;


        public delegate IAsyncEnumerable<ExecuteResult<TData>> ProduceHandler();
        public delegate ValueTask<ExecuteResult<TResult>> ProcessHandler(TData data);
        public delegate ValueTask LogHandler(Diagnostic diagnostic);

        private ProduceHandler? produce;
        private ProcessHandler? process;
        private LogHandler? log;

        private int totals;
        private int errors;

        internal FileProcessor()
        {
            dataChannelOptions = new BoundedChannelOptions(DefaultDataChannelCapacity)
            {
                SingleWriter = true,
                SingleReader = false,
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.Wait
            };
            messageChannelOptions = new BoundedChannelOptions(DefaultMessageChannelCapacity)
            {
                SingleWriter = false,
                SingleReader = true,
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.Wait
            };

            resultCollection = new();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void IncreaseTotals() => totals++;
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void IncreaseErrors() => errors++;

        public static FileProcessor<TData, TResult> Create() => new();

        public FileProcessor<TData, TResult> SetDataChannelCapacity(int capacity)
        {
            dataChannelOptions.Capacity = capacity;
            return this;
        }
        public FileProcessor<TData, TResult> SetMessageChannelCapacity(int capacity)
        {
            dataChannelOptions.Capacity = capacity;
            return this;
        }
        public FileProcessor<TData, TResult> SetMessageChannelMode(BoundedChannelFullMode mode)
        {
            dataChannelOptions.FullMode = mode;
            return this;
        }
        public FileProcessor<TData, TResult> SetWorkerCount(int count)
        {
            if (count is < -1 or 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            workerCount = count;

            return this;
        }
        public FileProcessor<TData, TResult> ConfigureProducer(ProduceHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            produce = handler;
            return this;
        }
        public FileProcessor<TData, TResult> ConfigureProcessor(ProcessHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            process = handler;
            return this;
        }
        public FileProcessor<TData, TResult> ConfigureLogger(LogHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            log = handler;
            return this;
        }

        public async Task<ProcessResult<IEnumerable<TResult>>> RunAsync()
        {
            if (produce is null)
                throw new NullReferenceException("Producer not configured");

            if (process is null)
                throw new NullReferenceException("Consimer not configured");

            if (log is null)
                throw new NullReferenceException("Logger not configured");

            dataChannel = Channel.CreateBounded<TData>(dataChannelOptions);
            messageChannel = Channel.CreateBounded<Diagnostic>(messageChannelOptions);

            if (workerCount == -1)
                workerCount = Environment.ProcessorCount;

            var logger = LogAsync().ConfigureAwait(false);

            var watch = System.Diagnostics.Stopwatch.StartNew();

            await Task.WhenAll(ConsumeParallelAsync(workerCount), ProduceAsync()).ConfigureAwait(false);

            watch.Stop();

            messageChannel.Writer.TryComplete();

            await logger;

            return new(resultCollection.ToList(), errors == 0, totals, errors, watch.Elapsed);
        }

        private async Task ProduceAsync()
        {
            var dataWriter = dataChannel!.Writer;
            var messageWriter = messageChannel!.Writer;

            await foreach (var data in produce!().ConfigureAwait(false))
            {
                IncreaseTotals();

                if (data.Success)
                {
                    await dataWriter.WriteAsync(data.Result!).ConfigureAwait(false);
                }
                else
                {
                    IncreaseErrors();
                }
                
                if (data.Diagnostic is not null)
                {
                    await messageWriter.WriteAsync(data.Diagnostic).ConfigureAwait(false);
                }
            }

            dataWriter.TryComplete();
        }
        private async ValueTask ConsumeAsync()
        {
            var dataReader = dataChannel!.Reader;
            var messageWriter = messageChannel!.Writer;

            await foreach (var data in dataReader.ReadAllAsync().ConfigureAwait(false))
            {
                var result = await process!(data).ConfigureAwait(false);

                if (result.Success)
                {
                    resultCollection.Add(result.Result!);
                }
                else
                {
                    IncreaseErrors();
                }

                if (result.Diagnostic is not null)
                {
                    await messageWriter.WriteAsync(result.Diagnostic).ConfigureAwait(false);
                }
            }
        }
       
        private Task ConsumeParallelAsync(int workers)
        {
            return Parallel.ForEachAsync(Enumerable.Range(1, workerCount), (_, cancelationToken) => ConsumeAsync());
        }
        private async Task LogAsync()
        {
            var messageReader = messageChannel!.Reader;

            await foreach (var data in messageReader.ReadAllAsync().ConfigureAwait(false))
            {
                await log!(data).ConfigureAwait(false);
            }
        }
    }
}
