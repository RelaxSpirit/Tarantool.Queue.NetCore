using ProGaudi.MsgPack.Light;

namespace Tarantool.Queues.Converters
{
    public abstract class ConverterBase<TEntity> : IMsgPackConverter<TEntity> where TEntity : class, new()
    {
        private MsgPackContext _context = null!;
        private bool _initialized;
        
        protected IMsgPackConverter<string> StringConverter { get; private set; } = null!;
        public void Initialize(MsgPackContext context)
        {
            _context = context;
        }

        public virtual TEntity Read(IMsgPackReader reader)
        {
            InitializeIfNeeded();

            var mapLength = reader.ReadMapLength();
            if (!mapLength.HasValue)
            {
                return null!;
            }

            var result = new TEntity();

            FillEntity(result, reader, mapLength.Value);

            return result;
        }

        public void Write(TEntity value, IMsgPackWriter writer)
        {
            throw new NotImplementedException();
        }

        protected void InitializeIfNeeded()
        {
            if (!_initialized)
            {
                _initialized = true;
                StringConverter = _context.GetConverter<string>();
                InitializeConverter(_context);
            }
        }

        protected abstract void InitializeConverter(MsgPackContext context);

        protected abstract void FillEntity(TEntity entity, IMsgPackReader reader, uint mapLength);
    }
}
