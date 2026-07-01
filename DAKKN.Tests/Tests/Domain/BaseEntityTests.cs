using DAKKN.Domain.Common;

namespace DAKKN.Tests.Tests.Domain
{
    public class BaseEntityTests
    {
        private class TestEntity : BaseEntity<Guid>
        {
            public string Name { get; set; } = string.Empty;
        }

        [Fact]
        public void Constructor_ShouldGenerateId_WhenGenericIsGuid()
        {
            var entity = new TestEntity();
            entity.Id.Should().NotBeEmpty();
            entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            entity.IsDeleted.Should().BeFalse();
            entity.IsActive.Should().BeTrue();
        }

        [Fact]
        public void MarkAsCreated_ShouldSetCreatedByAndCreatedAt()
        {
            var entity = new TestEntity();
            entity.MarkAsCreated("user123");

            entity.CreatedBy.Should().Be("user123");
            entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void MarkAsUpdated_ShouldSetUpdatedByAndUpdatedAt()
        {
            var entity = new TestEntity();
            entity.MarkAsUpdated("user456");

            entity.UpdatedBy.Should().Be("user456");
            entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void MarkAsDeleted_ShouldSetDeletedFlags()
        {
            var entity = new TestEntity();

            entity.MarkAsDeleted("user789");

            entity.IsDeleted.Should().BeTrue();
            entity.DeletedBy.Should().Be("user789");
            entity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void MarkAsRestored_ShouldClearDeletedFlags()
        {
            var entity = new TestEntity();
            entity.MarkAsDeleted("user789");
            entity.MarkAsRestored();

            entity.IsDeleted.Should().BeFalse();
            entity.DeletedBy.Should().BeNull();
            entity.DeletedAt.Should().BeNull();
        }
    }
}
