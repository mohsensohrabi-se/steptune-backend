using System;
using System.Collections.Generic;
using System.Text;

namespace StepTune.Domain.Common
{
    public abstract record DomainEvent(Guid EventId, DateTime OccuredOn)
    {
        protected DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }
    }
}
