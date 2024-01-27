using Tarantool.Queues.Options;

namespace Tarantool.Queues.Model
{
    public class TubeInfo
    {
        public string Engine { get; private set; } = string.Empty;
        public bool Temporary => CreationOptions.Temporary;
        public QueueType TubeType => CreationOptions.QueueType;
        public int? Capacity => CreationOptions.Capacity;
        public int TubeId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public TubeCreationOptions CreationOptions { get; private set; } = null!;

        public static async Task<TubeInfo> GetTubeInfo(IQueue queue, string tubeName)
        {
            return new TubeInfo()
            {
                Engine = await queue.GetTubeEngine(tubeName),
                Name = tubeName,
                CreationOptions = await queue.GetTubeCreationOptions(tubeName),
                TubeId = await queue.GetTubeId(tubeName)
            };
        }
    }
}
