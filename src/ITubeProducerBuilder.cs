using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public interface ITubeProducerBuilder
    {
        Task<IProducer<TTubeOptions>> Build<TTubeOptions>(string queueTubeName)
            where TTubeOptions : TubeOptions;
        Task<IProducer<TTubeOptions>> BuildCustomTubeProducer<TTubeOptions>(string queueTubeName)
            where TTubeOptions : AnyTubeOptions;
    }
}
