using KnowledgeKeeper.Domain.Common;
using KnowledgeKeeper.Domain.Organization;

namespace KnowledgeKeeper.Domain.Tests;

public class EmployeeTests
{
    [Fact]
    public void Create_refuse_un_email_invalide()
    {
        Assert.Throws<DomainException>(() =>
            Employee.Create("Karim Benali", "pas-un-email", Guid.NewGuid(), new DateOnly(2011, 3, 1)));
    }

    [Fact]
    public void AnnounceDeparture_refuse_une_date_anterieure_a_l_embauche()
    {
        var karim = Employee.Create("Karim Benali", "karim@example.com", Guid.NewGuid(), new DateOnly(2011, 3, 1));

        Assert.Throws<DomainException>(() => karim.AnnounceDeparture(new DateOnly(2010, 1, 1)));
    }

    [Fact]
    public void AnnounceDeparture_marque_l_employe_comme_sortant()
    {
        var karim = Employee.Create("Karim Benali", "karim@example.com", Guid.NewGuid(), new DateOnly(2011, 3, 1));

        karim.AnnounceDeparture(new DateOnly(2026, 12, 31));

        Assert.True(karim.IsLeaving);
    }
}
