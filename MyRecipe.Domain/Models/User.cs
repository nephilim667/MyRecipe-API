using MyRecipe.Domain.Enums;

namespace MyRecipe.Domain.Models
{
    public class User : BaseModel
    {
        public Guid Id { get; private set; }
        public UserStatus Status { get; private set; } = UserStatus.Inactive;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;


        // Domain constructor used when reconstructing the model from the database/infrastructure layer
        public User(Guid id, string email, UserStatus status, DateTime creationDate, DateTime? modificationDate = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            Id = id;
            Email = email.ToLowerInvariant().Trim();
            Status = status;
            CreationDate = creationDate;
            LastModificationDate = modificationDate;
        }

        // Pure business logic invariants executed within the domain boundary
        public void Suspend()
        {
            if (Status == UserStatus.Suspended) return;

            Status = UserStatus.Suspended;
            LastModificationDate = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (Status == UserStatus.Active) return;

            Status = UserStatus.Active;
            LastModificationDate = null;
        }
    }
}
