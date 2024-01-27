using Microsoft.Extensions.DependencyInjection;
using ProGaudi.Tarantool.Client.Model;

namespace Tarantool.Queues
{
    public static class DIExtension
    {
        public static IServiceCollection UseTarantoolQueue(this IServiceCollection services, string connectionString)
        {
            return services.UseTarantoolQueue(new ClientOptions(connectionString));
        }

        public static IServiceCollection UseTarantoolQueue(this IServiceCollection services, ClientOptions clientOptions)
        {
            return services.AddSingleton(_ => Queue.GetQueue(clientOptions));
        }

        public static IServiceCollection AddTubeConsumerBuilder(this IServiceCollection services)
        {
            return services.AddSingleton<ITubeConsumerBuilder, TubeConsumerBuilder>();
        }

        public static IServiceCollection AddTubeProducerBuilder(this IServiceCollection services)
        {
            return services.AddSingleton<ITubeProducerBuilder, TubeProducerBuilder>();
        }
    }
}
