using Tarantool.Queues.Options;

namespace Tarantool.Queues
{
    public interface ITubeProducerBuilder
    {
        Task<IProducer<TTubeOptions>> Build<TTubeOptions>(string queueTubeName)
            where TTubeOptions : TubeOptions;
        Task<TProducer?> BuildCustomTubeProducer<TProducer>(string queueTubeName)
            where TProducer : TubeProducer<AnyTubeOptions>;
    }
}
