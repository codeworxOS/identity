using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Codeworx.Identity
{
    public class UriBuilder
    {
        private ConcurrentDictionary<string, ConcurrentBag<string>> _query;

        private ConcurrentDictionary<string, ConcurrentBag<string>> _fragment;

        private ImmutableArray<string> _segments;

        public UriBuilder(string baseUrl)
            : this(new Uri(baseUrl))
        {
        }

        public UriBuilder(Uri baseUrl)
        {
            var uri = baseUrl;

            if (!uri.IsDefaultPort)
            {
                Port = uri.Port;
            }

            Schema = uri.Scheme;
            Host = uri.Host;
            _query = GetParameters(uri.Query.TrimStart('?'));
            _fragment = GetParameters(uri.Fragment.TrimStart('#'));

            _segments = ImmutableArray.CreateRange<string>(GetSegments(uri));
        }

        public UriBuilder(string schema, string host)
        {
            this.Schema = schema;
            this.Host = host;
            _segments = ImmutableArray<string>.Empty;
            _query = new ConcurrentDictionary<string, ConcurrentBag<string>>();
            _fragment = new ConcurrentDictionary<string, ConcurrentBag<string>>();
        }

        public string Host { get; set; }

        public int? Port { get; set; }

        public ImmutableDictionary<string, ImmutableArray<string>> Query => _query.ToImmutableDictionary(p => p.Key, p => p.Value.ToImmutableArray());

        public ImmutableDictionary<string, ImmutableArray<string>> Fragment => _fragment.ToImmutableDictionary(p => p.Key, p => p.Value.ToImmutableArray());

        public ImmutableArray<string> Segments => _segments;

        public string Schema { get; set; }

        public void AppendPath(string path)
        {
            var segments = path.Trim('/').Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            _segments = _segments.AddRange(segments);
        }

        public void AppendQueryParameter(string parameter, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            _query.GetOrAdd(parameter, p => new ConcurrentBag<string>())
                .Add(value);
        }

        public void AppendFragment(string parameter, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            _fragment.GetOrAdd(parameter, p => new ConcurrentBag<string>())
                .Add(value);
        }

        public override string ToString()
        {
            var result = $"{Schema}://{Host}";
            if (Port.HasValue)
            {
                result += $":{Port.Value}";
            }

            if (_segments.Any())
            {
                result += $"/{string.Join("/", _segments.Select(x => Uri.EscapeDataString(x)))}";
            }

            if (Query.Any(p => p.Value.Any()))
            {
                var queryParams = from q in Query
                                  from value in q.Value
                                  select $"{Uri.EscapeDataString(q.Key)}={Uri.EscapeDataString(value)}";

                result += $"?{string.Join("&", queryParams)}";
            }

            if (Fragment.Any(p => p.Value.Any()))
            {
                var queryParams = from q in Fragment
                                  from value in q.Value
                                  select $"{Uri.EscapeDataString(q.Key)}={Uri.EscapeDataString(value)}";

                result += $"#{string.Join("&", queryParams)}";
            }

            return result;
        }

        private static ConcurrentDictionary<string, ConcurrentBag<string>> GetParameters(string queryPart)
        {
            var queryParams = from q in queryPart.Split('&')
                              let splitted = q.Split('=')
                              where splitted.Length == 2 && !string.IsNullOrEmpty(splitted[1])
                              select new
                              {
                                  Parameter = Uri.UnescapeDataString(splitted[0]),
                                  Value = Uri.UnescapeDataString(splitted[1]),
                              };

            return new ConcurrentDictionary<string, ConcurrentBag<string>>(
                        queryParams
                            .GroupBy(p => p.Parameter)
                            .ToDictionary(p => p.Key, p => new ConcurrentBag<string>(p.Select(x => x.Value))));
        }

        private static IEnumerable<string> GetSegments(Uri uri)
        {
            return uri.Segments
                        .Select(p => p.Trim('/'))
                        .Where(p => !string.IsNullOrEmpty(p))
                        .Select(p => Uri.UnescapeDataString(p))
                        .ToList();
        }
    }
}
