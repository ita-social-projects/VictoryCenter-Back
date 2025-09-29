namespace VictoryCenter.BLL.Constants;

public static class ReorderConstants
{
    public static readonly int MaxElementsSwapCount = 500;

    public static readonly string ElementsPrioritiesIsNotInSeqentialOrder = "Priorities of the elements to be reordered are not sequential";

    public static string ErrorWithReordering(string message)
    {
        return $"Error with reordering: {message}";
    }

    public static string ExceededMaxElementsSwapCount(int actualCount)
    {
        return $"Exceeded max elements swap count. Provided: {actualCount}, Max: {MaxElementsSwapCount}";
    }

    public static string NotAllEntitiesFoundForReorder(int foundCount, int expectedCount)
    {
        return $"Not all entities found for reorder for the provided IDs order. Found: {foundCount}, Expected: {expectedCount}";
    }
}
