using StepTune.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepTune.Domain.Workouts.Events
{
    public sealed record WorkoutFinishedEvent(
        Guid SessionId,
        Guid UserId,
        double TotalDistance,
        int TotalSteps,
        double Calories
        ):DomainEvent;
    
}
