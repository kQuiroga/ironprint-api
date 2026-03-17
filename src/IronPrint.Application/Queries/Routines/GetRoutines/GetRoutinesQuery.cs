using IronPrint.Application.Common;

namespace IronPrint.Application.Queries.Routines.GetRoutines;

public record GetRoutinesQuery(string UserId) : IQuery<IEnumerable<RoutineDto>>;
