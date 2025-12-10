using StepTune.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepTune.Domain.Workouts.ValueObjects
{
    public sealed class Pace :IEquatable<Pace>
    {
        // It is calculate based on min/km
        // 3:00 min/km means it takes 3 min for a runner to cover a distance of 1km
        private static readonly TimeSpan Min = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan Max = TimeSpan.FromMinutes(30);

        public TimeSpan Value;

        private Pace(TimeSpan value)
        {
            Value = value;
        }

        public static Result<Pace> From(TimeSpan span)
        {
            if (span < Min || span > Max)
                return Result.Failure<Pace>($"Span must be between {Min} - {Max}");
            return Result.Success<Pace>(new Pace(span));
        }
        public static Result<Pace> FromMinutesAndSeconds(int minutes, int seconds)
        {
            var pace = new Pace(TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds));
            return Result.Success<Pace>(pace);
        }

        public bool Equals(Pace? other)
        {
            return other !=null && Value.Equals(other.Value);
        }

        public override bool Equals(object? obj)
        {
            return obj is Pace other && Equals(other);
        }

        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => $"{Minutes}:{Seconds:00} min/km";

        public int Minutes => (int)Value.Minutes;
        public int Seconds => (int)Value.Seconds;

    }
}
