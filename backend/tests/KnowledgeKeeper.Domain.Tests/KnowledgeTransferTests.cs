using KnowledgeKeeper.Domain.Common;
using KnowledgeKeeper.Domain.Organization;
using KnowledgeKeeper.Domain.Transfers;

namespace KnowledgeKeeper.Domain.Tests;

public class KnowledgeTransferTests
{
    private static readonly Guid PositionId = Guid.NewGuid();
    private static readonly DateOnly DepartureDate = new(2026, 12, 31);

    private static Employee DepartingKarim()
    {
        var karim = Employee.Create("Karim Benali", "karim@example.com", PositionId, new DateOnly(2011, 3, 1));
        karim.AnnounceDeparture(DepartureDate);
        return karim;
    }

    private static Employee Sarah() =>
        Employee.Create("Sarah Haddad", "sarah@example.com", PositionId, new DateOnly(2026, 11, 2));

    [Fact]
    public void Plan_cree_une_transmission_planifiee_a_la_date_de_depart()
    {
        var transfer = KnowledgeTransfer.Plan(DepartingKarim());

        Assert.Equal(TransferStatus.Planned, transfer.Status);
        Assert.Equal(DepartureDate, transfer.DueDate);
        Assert.Equal(PositionId, transfer.PositionId);
    }

    [Fact]
    public void Plan_refuse_un_employe_sans_date_de_depart()
    {
        Assert.Throws<DomainException>(() => KnowledgeTransfer.Plan(Sarah()));
    }

    [Fact]
    public void AssignSuccessor_refuse_que_l_employe_sortant_soit_son_propre_successeur()
    {
        var karim = DepartingKarim();
        var transfer = KnowledgeTransfer.Plan(karim);

        Assert.Throws<DomainException>(() => transfer.AssignSuccessor(karim));
    }

    [Fact]
    public void Complete_refuse_une_transmission_non_demarree()
    {
        var transfer = KnowledgeTransfer.Plan(DepartingKarim());

        Assert.Throws<DomainException>(transfer.Complete);
    }

    [Fact]
    public void Une_transmission_suit_le_cycle_Planned_InProgress_Completed()
    {
        var transfer = KnowledgeTransfer.Plan(DepartingKarim());
        transfer.AssignSuccessor(Sarah());

        transfer.Start();
        transfer.Complete();

        Assert.Equal(TransferStatus.Completed, transfer.Status);
        Assert.Throws<DomainException>(() => transfer.AssignSuccessor(Sarah()));
    }

    [Fact]
    public void Involves_reconnait_l_employe_sortant_et_le_successeur()
    {
        var karim = DepartingKarim();
        var sarah = Sarah();
        var transfer = KnowledgeTransfer.Plan(karim);
        transfer.AssignSuccessor(sarah);

        Assert.True(transfer.Involves(karim.Id));
        Assert.True(transfer.Involves(sarah.Id));
    }

    [Fact]
    public void Involves_ignore_un_employe_non_concerne()
    {
        var transfer = KnowledgeTransfer.Plan(DepartingKarim());

        Assert.False(transfer.Involves(Guid.NewGuid()));
    }
}
