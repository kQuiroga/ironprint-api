using IronPrint.Application.Common;
using IronPrint.Application.Queries.Routines;

namespace IronPrint.Application.Queries.Routines.GetActiveRoutine;

public record GetActiveRoutineQuery(string UserId) : IQuery<RoutineDto?>;
