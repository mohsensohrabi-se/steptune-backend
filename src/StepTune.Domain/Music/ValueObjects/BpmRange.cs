using StepTune.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepTune.Domain.Music.ValueObjects
{
    public sealed class BpmRange:IEquatable<BpmRange>
    {
        public int Min { get; }
        public int Max { get; }

        private BpmRange(int min, int max)
        {
            Min = min; 
            Max = max; 
        }

        public static Result<BpmRange> Create(int min, int max)
        {
            if (min < 0)
                return Result.Failure<BpmRange>("Min Range must be positive");
            if (min > max)
                return Result.Failure<BpmRange>("Min must be lower than the max Range");
            var bpmRange = new BpmRange(min, max);
            return Result.Success<BpmRange>(bpmRange);
        }

        public bool Contains(int bpm) => bpm >= Min && bpm <= Max;
        
        public int DistanceTo(int bpm)
        {
            if (Contains(bpm)) return 0;
            if (bpm < Min) return Min - bpm;
            return bpm - Max;
        }
        public bool Equals(BpmRange? other)
        {
            if(other == null) return false;
            return Min == other.Min && Max == other.Max;
        }

        public override bool Equals(object? obj) => obj is BpmRange other && Equals(other);
        public override int GetHashCode()
        {
            return HashCode.Combine(Min, Max);
        }
    }
}
