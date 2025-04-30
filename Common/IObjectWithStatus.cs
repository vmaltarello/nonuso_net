namespace Nonuso.Common
{
    public interface IObjectWithStatus
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        bool IsEnabled { get; set; }
    }

    public static class IObjectWithStatusUtils
    {
        public static void PopulateStatus(this IObjectWithStatus target, IObjectWithStatus source)
        {
            target.CreatedAt = source.CreatedAt;
            target.UpdatedAt = source.UpdatedAt;
            target.IsEnabled = source.IsEnabled;
        }

        public static void SetDefaultStatus(this IObjectWithStatus target)
        {
            target.CreatedAt = DateTime.UtcNow;
            target.UpdatedAt = DateTime.MinValue;
            target.IsEnabled = true;
        }
    }
}
