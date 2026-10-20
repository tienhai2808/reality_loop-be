using Application.AI.Services;
using Application.Common.Interfaces;

namespace Application.AI.Commands.DeleteAiConnection;

public sealed class DeleteAiConnectionCommandHandler(ICurrentUser current, IAiConnectionRepository connections)
{
    public Task Handle(DeleteAiConnectionCommand command, CancellationToken ct)
    {
        AiSetupValidation.Version(command.Version);
        return connections.DeleteAsync(current.UserId, command.Id, command.Version, ct);
    }
}
