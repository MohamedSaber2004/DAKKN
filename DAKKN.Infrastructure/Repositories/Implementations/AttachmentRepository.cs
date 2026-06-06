using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;

namespace DAKKN.Infrastructure.Repositories.Implementations
{
    public class AttachmentRepository: GenericRepository<Attachment, Guid>, IAttachmentRepository
    {

        public AttachmentRepository(DAKKNDbContext context) : base(context) { }
    }
}
