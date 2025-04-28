using Nonuso.Common;

namespace Nonuso.Application.DTOs.Base
{
    public abstract class DtoWithStatus : IObjectWithStatus
    {
        public virtual DateTime CreatedAt { get; set; }

        public virtual DateTime? UpdatedAt { get; set; }

        public virtual DateTime? DeletedAt { get; set; }

        public virtual bool IsEnabled { get; set; }
    }
}
