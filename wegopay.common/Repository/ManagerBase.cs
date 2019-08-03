using Microsoft.EntityFrameworkCore;

namespace wegopay.common.Repository
{
    public class ManagerBase<TEntity> : SimpleRepository<TEntity> where TEntity : class
    {
        public ManagerBase(DbContext context) : base(context)
        {
            
        }
    }
}
