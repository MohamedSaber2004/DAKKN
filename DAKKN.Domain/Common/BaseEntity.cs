using DAKKN.Domain.Common.Interfaces;

namespace DAKKN.Domain.Common
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; } = true;

        public void MarkAsCreated(string createdBy)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
        }

        public void MarkAsUpdated(string updatedBy)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }

        public void MarkAsDeleted(string deletedBy)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
        }
    }

    public abstract class BaseEntity<TKey> : BaseEntity, IBaseEntity<TKey>
    {
        public TKey Id { get; set; } = default!;

        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            if (typeof(TKey) == typeof(Guid) && Equals(Id, default(TKey)))
            {
                Id = (TKey)(object)Guid.NewGuid();
            }
        }
    }
}
