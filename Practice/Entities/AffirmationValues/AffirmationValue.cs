using AngularNetBase.Shared.Core.Domain;
using AngularNetBase.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Entities.AffirmationValues
{
    public class AffirmationValue : Entity<Guid>, IAggregateRoot
    {
        private readonly List<ValueStatement> _statements = new();

        public string Name { get; private set; } = string.Empty;

        public IReadOnlyCollection<ValueStatement> Statements => _statements;

        private AffirmationValue() : base() { }

        public AffirmationValue(Guid id, string name) : base(id)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Affirmation value name is required.", nameof(name));

            Name = name.Trim();
        }

        public ValueStatement AddStatement(Guid statementId, string text, bool isActive = true, string? textEn = null)
        {
            if (statementId == Guid.Empty)
                throw new ArgumentException("Statement id cannot be empty.", nameof(statementId));

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Statement text is required.", nameof(text));

            var statement = new ValueStatement(statementId, Id, text.Trim(), isActive, textEn);
            _statements.Add(statement);

            return statement;
        }

        public void ActivateStatement(Guid statementId)
        {
            var statement = _statements.FirstOrDefault(s => s.Id == statementId)
                ?? throw new InvalidOperationException("Value statement was not found.");

            statement.Activate();
        }

        public void DeactivateStatement(Guid statementId)
        {
            var statement = _statements.FirstOrDefault(s => s.Id == statementId)
                ?? throw new InvalidOperationException("Value statement was not found.");

            statement.Deactivate();
        }

    }
}
