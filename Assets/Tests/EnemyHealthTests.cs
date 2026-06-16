using NUnit.Framework;
using JuegoMental.Core;

public class EnemyHealthTests
{
    [Test]
    public void StartsFull_NotDead()
    {
        var h = new EnemyHealth(3f);
        Assert.AreEqual(1f, h.Fraction);
        Assert.IsFalse(h.IsDead);
    }

    [Test]
    public void Damage_ReducesHp()
    {
        var h = new EnemyHealth(4f);
        h.Damage(1f);
        Assert.AreEqual(0.75f, h.Fraction);
    }

    [Test]
    public void Damage_ToZero_IsDead()
    {
        var h = new EnemyHealth(2f);
        h.Damage(5f);
        Assert.AreEqual(0f, h.Fraction);
        Assert.IsTrue(h.IsDead);
    }
}
