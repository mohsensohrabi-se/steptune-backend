using System;
using System.Collections.Generic;
using System.Text;

namespace StepTune.Domain.Music.ValueObjects
{
    public sealed class BpmCategory
    {
        public string Value { get; }
        private BpmCategory(string value)
        {
            Value = value;
        }

        public static BpmCategory FromBpm(int bpm)
        {
            return bpm switch
            {
                <= 100 => new BpmCategory("relaxed"),
                <= 125 => new BpmCategory("moderate"),
                <= 150 => new BpmCategory("Fast"),
                _ => new BpmCategory("Intense")
            };
        }

        public override string ToString() => Value;
    }
}
