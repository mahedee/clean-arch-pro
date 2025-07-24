using EduTrack.Domain.Common;
using Xunit;

namespace EduTrack.Domain.UnitTests.Common
{
    /// <summary>
    /// Test entity for testing BaseEntity functionality
    /// </summary>
    public class TestEntity : BaseEntity<Guid>
    {
        public string Name { get; private set; } = string.Empty;

        public static TestEntity Create(string name)
        {
            return new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = name
            };
        }

        public void UpdateName(string newName)
        {
            Name = newName;
            MarkAsUpdated("TestUser");
        }
    }

    /// <summary>
    /// Test domain event for testing
    /// </summary>
    public class TestDomainEvent : DomainEvent
    {
        public string Message { get; }

        public TestDomainEvent(string message)
        {
            Message = message;
        }
    }

    public class BaseEntityTests
    {
        [Fact]
        public void Create_ShouldSetCreatedAt()
        {
            // Arrange & Act
            var entity = TestEntity.Create("Test");

            // Assert
            Assert.True(entity.CreatedAt <= DateTime.UtcNow);
            Assert.True(entity.CreatedAt > DateTime.UtcNow.AddSeconds(-1));
        }

        [Fact]
        public void MarkAsUpdated_ShouldSetUpdatedAt()
        {
            // Arrange
            var entity = TestEntity.Create("Test");
            var originalCreatedAt = entity.CreatedAt;

            // Act
            entity.MarkAsUpdated("TestUser");

            // Assert
            Assert.Equal(originalCreatedAt, entity.CreatedAt);
            Assert.NotNull(entity.UpdatedAt);
            Assert.Equal("TestUser", entity.UpdatedBy);
        }

        [Fact]
        public void MarkAsDeleted_ShouldSetDeletedAt()
        {
            // Arrange
            var entity = TestEntity.Create("Test");

            // Act
            entity.MarkAsDeleted("TestUser");

            // Assert
            Assert.NotNull(entity.DeletedAt);
            Assert.Equal("TestUser", entity.DeletedBy);
            Assert.True(entity.IsDeleted);
        }

        [Fact]
        public void Restore_ShouldClearDeletedFields()
        {
            // Arrange
            var entity = TestEntity.Create("Test");
            entity.MarkAsDeleted("TestUser");

            // Act
            entity.Restore();

            // Assert
            Assert.Null(entity.DeletedAt);
            Assert.Null(entity.DeletedBy);
            Assert.False(entity.IsDeleted);
            Assert.NotNull(entity.UpdatedAt);
        }

        [Fact]
        public void AddDomainEvent_ShouldAddEventToCollection()
        {
            // Arrange
            var entity = TestEntity.Create("Test");
            var domainEvent = new TestDomainEvent("Test message");

            // Act
            entity.AddDomainEvent(domainEvent);

            // Assert
            Assert.Single(entity.DomainEvents);
            Assert.Contains(domainEvent, entity.DomainEvents);
        }

        [Fact]
        public void RemoveDomainEvent_ShouldRemoveEventFromCollection()
        {
            // Arrange
            var entity = TestEntity.Create("Test");
            var domainEvent = new TestDomainEvent("Test message");
            entity.AddDomainEvent(domainEvent);

            // Act
            entity.RemoveDomainEvent(domainEvent);

            // Assert
            Assert.Empty(entity.DomainEvents);
        }

        [Fact]
        public void ClearDomainEvents_ShouldRemoveAllEvents()
        {
            // Arrange
            var entity = TestEntity.Create("Test");
            entity.AddDomainEvent(new TestDomainEvent("Test 1"));
            entity.AddDomainEvent(new TestDomainEvent("Test 2"));

            // Act
            entity.ClearDomainEvents();

            // Assert
            Assert.Empty(entity.DomainEvents);
        }

        [Fact]
        public void Equals_SameId_ShouldReturnTrue()
        {
            // Arrange
            var entity1 = TestEntity.Create("Test1");
            var entity2 = TestEntity.Create("Test2");
            
            // Use reflection to set the same ID for testing equality
            var id = entity1.Id;
            typeof(TestEntity).GetProperty("Id")!.SetValue(entity2, id);

            // Act & Assert
            Assert.True(entity1.Equals(entity2));
            Assert.True(entity1 == entity2);
            Assert.False(entity1 != entity2);
        }

        [Fact]
        public void Equals_DifferentId_ShouldReturnFalse()
        {
            // Arrange
            var entity1 = TestEntity.Create("Test1");
            var entity2 = TestEntity.Create("Test2");

            // Act & Assert
            Assert.False(entity1.Equals(entity2));
            Assert.False(entity1 == entity2);
            Assert.True(entity1 != entity2);
        }

        [Fact]
        public void GetHashCode_SameId_ShouldReturnSameHashCode()
        {
            // Arrange
            var entity1 = TestEntity.Create("Test1");
            var entity2 = TestEntity.Create("Test2");
            
            // Use reflection to set the same ID for testing equality
            var id = entity1.Id;
            typeof(TestEntity).GetProperty("Id")!.SetValue(entity2, id);

            // Act & Assert
            Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
        }
    }
}
