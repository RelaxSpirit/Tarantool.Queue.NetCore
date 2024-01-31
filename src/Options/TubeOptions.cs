using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Tarantool.Queues.Model;

namespace Tarantool.Queues.Options
{
    public abstract class TubeOptions : IDictionary<string, string>
    {
        private readonly Dictionary<string, string> _options = new();
        public abstract QueueType QueueType { get; protected set; }

        public string this[string key]
        {
            get => _options[key];
            set
            {
                ValidateOptionName(key);
                _options[key] = value;
            }
        }

        public ICollection<string> Keys => _options.Keys;

        public ICollection<string> Values => _options.Keys;

        public int Count => _options.Count;

        public bool IsReadOnly => false;

        public void Add(string key, string value)
        {
            ValidateOptionName(key);
            _options.Add(key, value);
        }

        public void Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _options.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return _options.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return _options.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            if (arrayIndex >= array.Length || arrayIndex < 0)
                throw new IndexOutOfRangeException($"Index value '{arrayIndex}' is outside of the array element count '{array.Length}'");

            for (; arrayIndex < array.Length; arrayIndex++)
            {
                _options[array[arrayIndex].Key] = array[arrayIndex].Value;
            }

        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var keyValuePair in _options)
                yield return keyValuePair;
        }

        public bool Remove(string key)
        {
            return _options.Remove(key);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            return _options.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _options.GetEnumerator();
        }

        protected abstract void ValidateOptionName(string optionName);

        protected TimeSpan? GetTimeSpanValue(string key)
        {
            TryGetValue(key, out string? value);
            return value is null ? null : TimeSpan.FromSeconds(double.Parse(value));
        }

        protected void SetTimeSpanValue(string key, TimeSpan? value)
        {
            if (value.HasValue)
                this[key] = value.Value.TotalSeconds.ToString();
            else
                Remove(key);
        }

        protected void ThrowValidateOptionNameException(string optionName)
        {
            throw new NotSupportedException($"Option '{optionName}' not supported by queue tube type '{QueueType}'");
        }

        protected void ThrowValidateCreationOptionNameException(string optionName)
        {
            throw new NotSupportedException($"Creation option '{optionName}' not supported by queue tube type '{QueueType}'");
        }

        public static TubeOptions GetDefaultTubeOptions(QueueType queueType, string? utubeName = null)
        {
            return queueType switch
            {
                QueueType.fifo => new FiFoTubeOptions(),
                QueueType.fifottl => new FiFoTtlTubeOptions(),
                QueueType.limfifottl => new LimFiFoTtlTubeOptions(),
                QueueType.utube => new UTubeTubeOptions(utubeName),
                QueueType.utubettl => new UTubeTtlTubeOptions(utubeName),
                _ => throw new NotSupportedException($"Queue tube type '{queueType}' not supported"),
            } ;
        }

        public override string ToString()
        {
            if (this.Count != 0)
            {
                var optString = string.Join(", ", this.Select(c => $"{c.Key} = {c.Value}"));
                return $"{{{optString}}}";
            }
            else
                return string.Empty;
        }
    }
}
