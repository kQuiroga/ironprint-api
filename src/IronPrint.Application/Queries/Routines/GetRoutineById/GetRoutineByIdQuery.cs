using IronPrint.Application.Common;

namespace IronPrint.Application.Queries.Routines.GetRoutineById;

public record GetRoutineByIdQuery(Guid Id, string UserId) : IQuery<RoutineDto>;
