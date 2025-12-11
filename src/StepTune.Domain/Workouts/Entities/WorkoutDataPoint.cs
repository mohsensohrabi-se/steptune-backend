using StepTune.Domain.Common;
using StepTune.Domain.Workouts.ValueObjects;


namespace StepTune.Domain.Workouts.Entities
{
    public class WorkoutDataPoint: AggregateRoot
    {
        private WorkoutDataPoint() { }
        public  DateTime TimeStamp { get; private set; }
        public Cadence? Cadence { get; private set; }
        public Pace? Pace { get; private set; }
        public GeoPoint? Location { get; private set; }
        public int steps { get; private set; }

        private WorkoutDataPoint(DateTime timeStamp, int steps, Cadence? cadence, Pace? pace, GeoPoint? location)
        {
            TimeStamp = timeStamp;
            Cadence = cadence;
            Pace = pace;
            Location = location;
            this.steps = steps;
        }
        public static WorkoutDataPoint Create(
            DateTime time,
            Cadence? cadence,
            Pace? pace,
            GeoPoint location,
            int steps)
        {
            return new WorkoutDataPoint(time, steps,cadence, pace, location);
        }
    }
}
