using StepTune.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepTune.Domain.Workouts.ValueObjects
{
    public sealed class Cadence : IEquatable<Cadence>
    {
        public int StepsPerMinute { get; }

        private const int Min = 10;
        private const int Max = 300;

        //SPM: Steps Per Minute
        private Cadence(int spm)
        {
            StepsPerMinute = spm;
        }

        public  static Result<Cadence> Create(int spm)
        {
            if (spm < Min || spm > Max)
                return Result.Failure<Cadence>($"SPM must be between {Min} - {Max}");
            return Result.Success<Cadence>(new Cadence(spm));
        }
        public bool Equals(Cadence? other)
        {
            if (other == null) return false;
            return StepsPerMinute == other.StepsPerMinute;
        }
        public override bool Equals(object? obj)
        {
            return obj is Cadence other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StepsPerMinute.GetHashCode();
        }

        public override string ToString()
        {
            return $"{StepsPerMinute} spm";
        }
    }
}
