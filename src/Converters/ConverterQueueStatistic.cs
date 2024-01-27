using ProGaudi.MsgPack.Light;
using Tarantool.Queues.Converters;

namespace Tarantool.Queues.Model
{
    public partial class QueueStatistic
    {
        public class Converter : ConverterBase<QueueStatistic>
        {
            private IMsgPackConverter<QueueTubeStatistic> _queueTubeStatisticConverter = null!;

            protected override void FillEntity(QueueStatistic entity, IMsgPackReader reader, uint mapLength)
            {
                for (var i = 0; i < mapLength; i++)
                {
                    entity.QueueTubesStatistic.Add(StringConverter.Read(reader), _queueTubeStatisticConverter.Read(reader));
                }
            }

            protected override void InitializeConverter(MsgPackContext context)
            {
                _queueTubeStatisticConverter = new QueueTubeStatistic.Converter();
                _queueTubeStatisticConverter.Initialize(context);
            }
        }
    }
}
