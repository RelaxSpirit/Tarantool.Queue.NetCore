using ProGaudi.MsgPack.Light;
using Tarantool.Queues.Converters;

namespace Tarantool.Queues.Model
{
    public partial class TubeTask
    {
        const int ITEM_COUNT = 3;
        public class Converter : ConverterBase<TubeTask>
        {
            private IMsgPackConverter<ulong> _ulongConverter = null!;

            public override TubeTask Read(IMsgPackReader reader)
            {
                InitializeIfNeeded();
                var arrayLength = reader.ReadArrayLength();
                if (arrayLength.HasValue)
                {
                    var entity = new TubeTask();
                    FillEntity(entity, reader, arrayLength.Value);
                    return entity;
                }
                else
                    return null!;
            }

            protected override void FillEntity(TubeTask entity, IMsgPackReader reader, uint mapLength)
            {
                if(mapLength != ITEM_COUNT)
                    throw new ArgumentException($"Invalid Task tuple length: {ITEM_COUNT} is expected, but got {mapLength}.");
                
                entity.TaskId = _ulongConverter.Read(reader);
                var stateString = StringConverter.Read(reader);
                entity.TaskState = (QueueTaskState) Convert.ToUInt16(stateString.ToCharArray()[0]);
                entity.TaskData = StringConverter.Read(reader);
            }

            protected override void InitializeConverter(MsgPackContext context)
            {
                _ulongConverter = context.GetConverter<ulong>();
            }
        }
    }
}
