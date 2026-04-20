using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Dtos.AffirmationValues
{
    public record AddStatementDto(string Text, bool IsActive = true, string? TextEn = null);
}
