using AngularNetBase.Shared.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.AffirmationValues
{
    public class ValueStatement : Entity<Guid>
    {
        public Guid AffirmationValueId { get; private set; }

        public string Text { get; private set; } = string.Empty;
        public string? TextEn { get; private set; }

        public bool IsActive { get; private set; }

        private ValueStatement() : base() { }

        internal ValueStatement(Guid id, Guid affirmationValueId, string text, bool isActive, string? textEn = null) : base(id)
        {
            if (affirmationValueId == Guid.Empty)
                throw new ArgumentException("Affirmation value id cannot be empty.", nameof(affirmationValueId));

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Statement text is required.", nameof(text));

            AffirmationValueId = affirmationValueId;
            Text = text.Trim();
            TextEn = string.IsNullOrWhiteSpace(textEn) ? null : textEn.Trim();
            IsActive = isActive;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
