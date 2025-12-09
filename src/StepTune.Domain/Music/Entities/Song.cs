using StepTune.Domain.Common;
using StepTune.Domain.Music.Events;
using StepTune.Domain.Music.ValueObjects;
using StepTune.Domain.Users.Entities;


namespace StepTune.Domain.Music.Entities
{
    public sealed class Song:AggregateRoot
    {
        private Song() { }
        public Guid? UserId { get; private set;  }
        public string Title { get; private set; } = default!;
        public string Artist { get; private set; } = default!;
        public int Duration { get; private set; }
        public int Bpm { get; private set; }
        public BpmRange BpmRange { get; private set; } = default!;
        public BpmCategory TempoCategory { get; private set; } = default!;
        public SongFile File { get; private set; } = default!;
        public DateTime UploadedAt { get; private set; }

        //Navigation prop
        public User? User { get; private set; }

        public static Result<Song> Upload(
            Guid userId,
            string title,
            string artist,
            int duration,
            int bpm,
            SongFile file)
        {
            if (userId == Guid.Empty)
                return Result.Failure<Song>("User ID  can not be empty");
            if (string.IsNullOrWhiteSpace(title))
                return Result.Failure<Song>("Title can not be empty");
            if (string.IsNullOrWhiteSpace(artist))
                return Result.Failure<Song>("Artist can not be empty");
            if (duration < 0)
                return Result.Failure<Song>("Duration must be greater than 0");
            if (bpm < 0)
                return Result.Failure<Song>("BPM must be greater than 0");
            var bpmResult = BpmRange.Create(bpm - 5, bpm + 5);
            if (bpmResult.IsFailure)
                return Result.Failure<Song>(bpmResult.Error);

            var song = new Song
            {
                Id = userId,
                Title = title,
                Artist = artist,
                Duration = duration,
                BpmRange = bpmResult.Value,
                TempoCategory = BpmCategory.FromBpm(bpm),
                File = file,
                UploadedAt = DateTime.UtcNow
            };

            song.AddDomainEvent(new SongUploadedEvent(song.Id,userId));
            return Result.Success<Song>(song);

        }

        
    }
}
