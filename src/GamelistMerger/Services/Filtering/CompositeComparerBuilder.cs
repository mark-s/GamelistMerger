using GamelistMerger.Models;

namespace GamelistMerger.Services.Filtering;

public sealed class CompositeComparerBuilder
{
    private readonly List<Func<Game, string?>> _selectors = [];

    public CompositeComparerBuilder ByPath()
    {
        _selectors.Add(g => g.Path);
        return this;
    }

    public CompositeComparerBuilder ById()
    {
        _selectors.Add(g => g.Id);
        return this;
    }


    public CompositeComparerBuilder ByCrc32()
    {
        _selectors.Add(g => g.Crc32);
        return this;
    }


    public CompositeComparerBuilder ByHash()
    {
        _selectors.Add(g => g.Hash);
        return this;
    }

    public IEqualityComparer<Game> Build()
    {
        if (_selectors.Count == 0)
            throw new InvalidOperationException("At least one property must be specified.");

        return new CompositeComparer([.. _selectors]);
    }

    private sealed class CompositeComparer(Func<Game, string?>[] selectors) : IEqualityComparer<Game>
    {
        public bool Equals(Game? x, Game? y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x is null || y is null)
                return false;

            foreach (var selector in selectors)
            {
                var xValue = selector(x);
                var yValue = selector(y);

                // Skip if either value is null/empty - try next selector
                if (string.IsNullOrEmpty(xValue) || string.IsNullOrEmpty(yValue))
                    continue;

                // Both have values - compare them
                return string.Equals(xValue, yValue, StringComparison.OrdinalIgnoreCase);
            }

            // No comparable properties found
            return false;
        }

        public int GetHashCode(Game obj)
        {
            // Use the first non-null/empty property for hashing
            foreach (var selector in selectors)
            {
                var value = selector(obj);
                if (!string.IsNullOrEmpty(value))
                    return value.ToUpperInvariant().GetHashCode();
            }

            return 0;
        }
    }
}