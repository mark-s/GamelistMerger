namespace GamelistMerger.Models;

public static class FolderEqualityComparers
{
    public static IEqualityComparer<Folder> ByPath { get; } = new PropertyComparer(f => f.Path);
    public static IEqualityComparer<Folder> ByName { get; } = new PropertyComparer(f => f.Name);
    public static IEqualityComparer<Folder> ById { get; } = new PropertyComparer(f => f.Id, StringComparison.Ordinal);

    private sealed class PropertyComparer(Func<Folder, string?> selector, StringComparison comparison = StringComparison.OrdinalIgnoreCase) : IEqualityComparer<Folder>
    {
        public bool Equals(Folder? x, Folder? y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null)
                return false;
            return string.Equals(selector(x), selector(y), comparison);
        }

        public int GetHashCode(Folder obj)
        {
            var value = selector(obj);
            if (value is null)
                return 0;
            return comparison == StringComparison.Ordinal
                ? value.GetHashCode()
                : value.ToUpperInvariant().GetHashCode();
        }
    }
}
