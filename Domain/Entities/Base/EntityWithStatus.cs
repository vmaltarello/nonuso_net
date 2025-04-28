using Nonuso.Common;

namespace Nonuso.Domain.Entities.Base
{
    public abstract class EntityWithStatus : EntityLog, IObjectWithStatus
    {
        public new void SetDefaults()
        {
            base.SetDefaults();
            this.SetDefaultStatus();
        }
    }
}
