using ProGaudi.MsgPack.Light;
using Tarantool.Queues.Converters;

namespace Tarantool.Queues.Model
{
    public partial class QueueTubeStatistic
    {
        public class Converter : ConverterBase<QueueTubeStatistic>
        {
            private IMsgPackConverter<Tasks> _tasksConverter = null!;
            private IMsgPackConverter<Calls> _callsConverter = null!;
            protected override void FillEntity(QueueTubeStatistic entity, IMsgPackReader reader, uint mapLength)
            {
                for (var i = 0; i < mapLength; i++)
                {
                    switch (StringConverter.Read(reader))
                    {
                        case "tasks":
                            entity.TasksInfo = _tasksConverter.Read(reader);
                            break;
                        case "calls":
                            entity.CallsInfo = _callsConverter.Read(reader);
                            break;
                        default:
                            reader.SkipToken();
                            break;
                    }
                }
            }
            protected override void InitializeConverter(MsgPackContext context)
            {
                _tasksConverter = new Tasks.Converter();
                _callsConverter = new Calls.Converter();
                _tasksConverter.Initialize(context);
                _callsConverter.Initialize(context);
            }
        }

        public partial class Calls
        {
            public class Converter : ConverterBase<Calls>
            {
                private IMsgPackConverter<ulong> _ulongConverter = null!;

                protected override void FillEntity(Calls entity, IMsgPackReader reader, uint mapLength)
                {
                    for (var i = 0; i < mapLength; i++)
                    {
                        switch (StringConverter.Read(reader))
                        {
                            case "ttr":
                                entity.Ttr = _ulongConverter.Read(reader);
                                break;
                            case "ttl":
                                entity.Ttl = _ulongConverter.Read(reader);
                                break;
                            case "put":
                                entity.Put = _ulongConverter.Read(reader);
                                break;
                            case "take":
                                entity.Take = _ulongConverter.Read(reader);
                                break;
                            case "ask":
                                entity.Ack = _ulongConverter.Read(reader);
                                break;
                            case "kick":
                                entity.Kick = _ulongConverter.Read(reader);
                                break;
                            case "bury":
                                entity.Bury = _ulongConverter.Read(reader);
                                break;
                            case "delay":
                                entity.Delay = _ulongConverter.Read(reader);
                                break;
                            case "delete":
                                entity.Delete = _ulongConverter.Read(reader);
                                break;
                            case "release":
                                entity.Release = _ulongConverter.Read(reader);
                                break;
                            case "touch":
                                entity.Touch = _ulongConverter.Read(reader);
                                break;
                            default:
                                reader.SkipToken();
                                break;
                        }
                    }
                }
                protected override void InitializeConverter(MsgPackContext context)
                {
                    _ulongConverter = context.GetConverter<ulong>();
                }
            }
        }

        public partial class Tasks
        {
            public class Converter : ConverterBase<Tasks>
            {
                private IMsgPackConverter<ulong> _ulongConverter = null!;
                protected override void FillEntity(Tasks entity, IMsgPackReader reader, uint mapLength)
                {
                    for (var i = 0; i < mapLength; i++)
                    {
                        switch (StringConverter.Read(reader))
                        {
                            case "taken":
                                entity.Taken = _ulongConverter.Read(reader);
                                break;
                            case "done":
                                entity.Done = _ulongConverter.Read(reader);
                                break;
                            case "ready":
                                entity.Ready = _ulongConverter.Read(reader);
                                break;
                            case "total":
                                entity.Total = _ulongConverter.Read(reader);
                                break;
                            case "delayed":
                                entity.Delayed = _ulongConverter.Read(reader);
                                break;
                            case "buried":
                                entity.Buried = _ulongConverter.Read(reader);
                                break;
                            default:
                                reader.SkipToken();
                                break;
                        }
                    }
                }

                protected override void InitializeConverter(MsgPackContext context)
                {
                    _ulongConverter = context.GetConverter<ulong>();
                }
            }
        }
    }
}
