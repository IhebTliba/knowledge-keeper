namespace KnowledgeKeeper.Domain.Common;

/// <summary>Levée quand une règle métier est violée.</summary>
public sealed class DomainException(string message) : Exception(message)
{
}
