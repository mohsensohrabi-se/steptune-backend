using StepTune.Domain.Common;
using StepTune.Domain.Users.Events;
using StepTune.Domain.Users.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepTune.Domain.Users.Entities
{
    public sealed class User : AggregateRoot
    {
        private User() { }
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public UserProfile Profile { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }

        public static Result<User> Register(
            string email,
            string passwordHash,
            UserProfile profile
            )
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure<User>("Email is required");
            if (string.IsNullOrWhiteSpace(passwordHash))
                return Result.Failure<User>("Password hash is required");

            var user = new User
            {
                Email = email.Trim().ToLower(),
                PasswordHash = passwordHash,
                Profile = profile,
                CreatedAt = DateTime.UtcNow
            };

            user.AddDomainEvent(new UserRegisteredEvent(user.Id));

            return Result.Success<User>(user);
        }

        public Result UpdateProfile(UserProfile newProfile)
        {
            Profile = newProfile;
            return Result.Success();
        }
    }
}
