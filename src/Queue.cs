using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using Tarantool.Queues.Model;
using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public partial class Queue : IQueue, IAdminQueue
    {
        const string REQUIRE = "queue";
        private bool disposedValue;
        private readonly Box _netBox;

        internal Queue(ClientOptions clientOptions) :
            this(new Box(clientOptions), clientOptions)
        {
            _netBox.Connect().GetAwaiter().GetResult();
            var versionResponse = _netBox.Eval<string>($"return {REQUIRE}._VERSION").GetAwaiter().GetResult();
            Version = versionResponse.Data[0];
            SessionUUid = _netBox.Info.Uuid;
        }

        Queue(Box netBox, ClientOptions clientOptions)
        {
            _netBox = netBox;
            clientOptions.MsgPackContext.RegisterConverter(new QueueStatistic.Converter());
            clientOptions.MsgPackContext.RegisterConverter(new TubeCreationOptions.Converter());
            clientOptions.MsgPackContext.RegisterConverter(new TubeTask.Converter());
        }

        public async Task<QueueState> GetState()
        {
           return Enum.Parse<QueueState> ((await _netBox.Eval<string>($"return {REQUIRE}.state()")).Data.First());
        }
        public Guid SessionUUid { get; private set; }
        public string Version { get; private set; } = string.Empty;
        public static IQueue GetQueue(string connectionString)
        {
            var clientOptions = new ClientOptions(connectionString);

            return GetQueue(clientOptions);
        }
        public static IAdminQueue GetAdminQueue(string connectionString)
        {
            var clientOptions = new ClientOptions(connectionString);

            return GetAdminQueue(clientOptions);
        }
        public static IQueue GetQueue(ClientOptions clientOptions)
        {
            return GetQueueObject(clientOptions);
        }
        public static IAdminQueue GetAdminQueue(ClientOptions clientOptions)
        {
            return GetQueueObject(clientOptions);
        }
        public async Task<QueueStatistic> GetStatistics()
        {
            var response = await _netBox.Eval<QueueStatistic>($"return {REQUIRE}.statistics()");
            return response.Data.First();
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _netBox.Dispose();
                }
                disposedValue = true;
            }
        }
        private static Queue GetQueueObject(ClientOptions clientOptions)
        {
            return new Queue(clientOptions);
        }

    }
}
