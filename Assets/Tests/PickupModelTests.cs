using NUnit.Framework;
using JuegoMental.Core;

public class PickupModelTests
{
    [Test]
    public void Good_AppliesNegativeDelta()
    {
        var m = new CortisolModel(100f);
        m.Add(50f);
        var p = new PickupModel(PickupKind.Good, 20f);
        p.ApplyTo(m);
        Assert.AreEqual(30f, m.Value);
    }

    [Test]
    public void Bad_AppliesPositiveDelta()
    {
        var m = new CortisolModel(100f);
        var p = new PickupModel(PickupKind.Bad, 20f);
        p.ApplyTo(m);
        Assert.AreEqual(20f, m.Value);
    }
}
