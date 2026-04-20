using AngularNetBase.Shared.Core.Domain;
using AngularNetBase.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.GrowthMessages
{
    public class GrowthMessage : Entity<Guid>, IAggregateRoot
    {
        public string Text { get; private set; } = string.Empty;
        public string? TextEn { get; private set; }
        public GrowthMessageType Type { get; private set; }
        public Guid? AffirmationValueId { get; private set; }

        public bool IsActive { get; private set; }

        private GrowthMessage() : base() { }

        public GrowthMessage(
            Guid id,
            string text,
            GrowthMessageType type = GrowthMessageType.Begin,
            bool isActive = true,
            Guid? affirmationValueId = null,
            string? textEn = null) : base(id)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Growth message text is required.", nameof(text));

            Text = text.Trim();
            TextEn = NormalizeOptional(textEn);
            Type = type;
            IsActive = isActive;
            AffirmationValueId = affirmationValueId;
        }

        private static string? NormalizeOptional(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
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
