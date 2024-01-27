using ProGaudi.Tarantool.Client.Model;
using Tarantool.Queues.Model;
using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public partial class Queue
    {
        public async Task<Tube> CreateTube(string queueName, TubeCreationOptions options)
        {
            
            var optionsString = options.ToString();
            
            if (!string.IsNullOrEmpty(optionsString))
                optionsString = ", " + optionsString;

            var callString = $"{REQUIRE}.create_tube('{queueName}', '{options.QueueType}'{optionsString})";

            await _netBox.Eval<TarantoolTuple>(callString);

            var tubeInfo = await TubeInfo.GetTubeInfo(this, queueName);

            return new Tube(this, tubeInfo);
        }

        public async Task<TubeCreationOptions> GetTubeCreationOptions(string tubeName)
        {
            var strRequest = $"return {REQUIRE}.tube.{tubeName}.type";
            var response = await _netBox.Eval<string>(strRequest);
            var tubeType = Enum.Parse<QueueType>(response.Data[0]);

            strRequest = $"return {REQUIRE}.tube.{tubeName}.opts";
            var optResponse = (await _netBox.Eval<TubeCreationOptions>(strRequest)).Data[0];
            optResponse.SetQueueType(tubeType);
            return optResponse;
        }

        public async Task<string> GetTubeEngine(string tubeName)
        {
            var strRequest = $"return {REQUIRE}.tube.{tubeName}.raw.space.engine";
            var response = await _netBox.Eval<string>(strRequest);
            return response.Data[0];
        }

        public async Task<int> GetTubeId(string tubeName)
        {
            return (await _netBox.Eval<int>($"return {REQUIRE}.tube.{tubeName}.tube_id")).Data[0];
        }

        public async Task<Tube> GetTube(string queueName)
        {
            var tubeInfo = await TubeInfo.GetTubeInfo(this, queueName);

            return new Tube(this, tubeInfo);
        }

        public async Task DeleteTube(string tubeName)
        {
            await _netBox.Eval<string>($"{REQUIRE}.tube.{tubeName}:drop()");
        }

        public class Tube : ITube
        {
            private readonly string _driverCallPath;
            private readonly Queue _parentShema;
            private readonly TubeInfo _tubeInfo;
            internal Tube(Queue queue, TubeInfo tubeInfo)
            {
                _parentShema = queue;
                _tubeInfo = tubeInfo;
                _driverCallPath = $"{REQUIRE}.tube.{Name}:";
            }
            public string Name => _tubeInfo.Name;
            public QueueType TubeType => _tubeInfo.TubeType;
            public TubeCreationOptions CreationOptions => _tubeInfo.CreationOptions;

            public async Task<QueueTubeStatistic> GetStatistics()
            {
                var response = await _parentShema._netBox.Eval<QueueStatistic>($"return {REQUIRE}.statistics({Name})");
                return response.Data.First().QueueTubesStatistic[Name];
            }
            public async Task<TubeTask> Put(string data, TubeOptions opts)
            {
                var optsSting = opts.ToString();
                if(!string.IsNullOrEmpty(optsSting))
                    optsSting = $", {optsSting}";
                var requestString = $"return {_driverCallPath}put('{data}'{optsSting})";
                return await GetTubeTask(requestString);
            }

            public async Task<TubeTask?> Take(int? timeout, CancellationToken cancellationToken)
            {
                var responseTask = _parentShema._netBox.Eval<TubeTask>($"return {_driverCallPath}take({(timeout?.ToString() ?? string.Empty)})");

                var response = await responseTask.WaitAsync(cancellationToken);

                return response.Data.FirstOrDefault();
            }
            
            public async Task<TubeTask> Ack(ulong taskId)
            {
                var requestString = $"return {_driverCallPath}ack({taskId})";
                return await GetTubeTask(requestString);
            }

            public async Task<TubeTask> Release(ulong task_id, TubeOptions opts)
            {
                var optsSting = opts.ToString();
                if (!string.IsNullOrEmpty(optsSting))
                    optsSting = $", {optsSting}";
                var requestString = $"return {_driverCallPath}release({task_id}{optsSting})";
                return await GetTubeTask(requestString);
            }

            public async Task<TubeTask> Peek(ulong taskId)
            {
                var requestString = $"return {_driverCallPath}peek({taskId})";

                return await GetTubeTask(requestString);
            }

            public async Task<TubeTask> Bury(ulong taskId)
            {
                var requestString = $"return {_driverCallPath}bury({taskId})";
                return await GetTubeTask(requestString);
            }

            public async Task<ulong> Kick(ulong count)
            {
                var requestString = $"return {_driverCallPath}kick({count})";
                var response = await _parentShema._netBox.Eval<ulong>(requestString);

                return response.Data[0];
            }

            public async Task<TubeTask> Touch(ulong taskId, TimeSpan delta)
            {
                var requestString = $"return {_driverCallPath}touch({taskId}, {delta.TotalSeconds})";
                return await GetTubeTask(requestString);
            }

            private async Task<TubeTask> GetTubeTask(string requestString)
            {
                var response = await _parentShema._netBox.Eval<TubeTask>(requestString);

                return response.Data[0];
            }

            public async Task<TubeTask> Delete(ulong taskId)
            {
                var requestString = $"return {_driverCallPath}delete({taskId})";
                return await GetTubeTask(requestString);
            }

            public async Task ReleaseAll()
            {
                var requestString = $"{_driverCallPath}release_all()";
                await _parentShema._netBox.Eval<TarantoolTuple>(requestString);
            }
        }
    }
}
