using Tarantool.Queues.Converters;
using ProGaudi.MsgPack.Light;

namespace Tarantool.Queues.Options
{
    public partial class TubeCreationOptions
    {
        public class Converter : ConverterBase<TubeCreationOptions>
        {
            private IMsgPackConverter<bool> _boolConverter = null!;
            private IMsgPackConverter<int> _intConverter = null!;
            protected override void FillEntity(TubeCreationOptions entity, IMsgPackReader reader, uint mapLength)
            {
                for (var i = 0; i < mapLength; i++)
                {
                    switch (StringConverter.Read(reader))
                    {
                        case TubeCreationOptions.IF_NOT_EXISTS:
                            entity.IfNotExists = _boolConverter.Read(reader);
                            break;
                        case TubeCreationOptions.CAPACITY:
                            entity.Capacity = _intConverter.Read(reader);
                            break;
                        case TubeCreationOptions.TEMPORARY:
                            entity.Temporary = _boolConverter.Read(reader);
                            break;
                        default:
                            reader.SkipToken();
                            break;
                    }
                }
            }

            protected override void InitializeConverter(MsgPackContext context)
            {
                _boolConverter = context.GetConverter<bool>();
                _intConverter = context.GetConverter<int>();
            }
        }
    }
}
