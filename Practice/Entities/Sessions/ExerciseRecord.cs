using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.Sessions
{
    public class ExerciseRecord : SessionEvent
    {
        public Guid ExerciseId { get; private set; }
        public ExerciseType Type { get; private set; }

        private ExerciseRecord() { }

        public ExerciseRecord(Guid exerciseId, ExerciseType type, DateTime timestamp) : base(timestamp)
        {
            if (exerciseId == Guid.Empty)
                throw new ArgumentException("ExerciseId ne može biti prazan.", nameof(exerciseId));

            ExerciseId = exerciseId;
            Type = type;
        }
    }
}
