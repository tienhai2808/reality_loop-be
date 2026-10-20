using Domain.AI;

namespace Application.AI.Models;

public sealed record AiCreationReservation(bool Acquired, AiConnectionDto? Completed);
