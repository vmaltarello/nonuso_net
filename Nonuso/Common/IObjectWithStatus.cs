namespace Nonuso.Common
{
    public interface IObjectWithStatus
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        DateTime? DeletedAt { get; set; }
        bool IsEnabled { get; set; }
    }

    public static class IObjectWithStatusUtils
    {
        public static void PopulateStatus(this IObjectWithStatus target, IObjectWithStatus source)
        {
            target.CreatedAt = source.CreatedAt;
            target.UpdatedAt = source.UpdatedAt;
            target.DeletedAt = source.DeletedAt;
            target.IsEnabled = source.IsEnabled;
        }

        public static void SetDefaultStatus(this IObjectWithStatus target)
        {
            target.CreatedAt = DateTime.Now;
            target.UpdatedAt = DateTime.MinValue;
            target.DeletedAt = DateTime.MinValue;
            target.IsEnabled = true;
        }
    }
}
