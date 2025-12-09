using StepTune.Domain.Music.Entities;
using StepTune.Domain.Music.ValueObjects;

namespace StepTune.Domain.Music.Services
{
    public sealed class BpmMatchingService
    {
        public IEnumerable<Song> MatchToBpm(IEnumerable<Song> songs, int targetBpm)
        {
            return songs
                .OrderBy(song => Math.Abs(song.Bpm - targetBpm))
                .ThenBy(song => song.Duration);
        }

        public IEnumerable<Song> MatchToRange(IEnumerable<Song> songs, BpmRange range)
        {
            return songs
                .Where(song => range.Contains(song.Bpm))
                .OrderBy(song => range.DistanceTo(song.Bpm));
        }
    }
}
