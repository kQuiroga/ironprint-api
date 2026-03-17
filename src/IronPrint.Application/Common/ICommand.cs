using IronPrint.Domain.Common;
using MediatR;

namespace IronPrint.Application.Common;

public interface ICommand : IRequest<Result> { }
public interface ICommand<T> : IRequest<Result<T>> { }
public interface IQuery<T> : IRequest<Result<T>> { }
