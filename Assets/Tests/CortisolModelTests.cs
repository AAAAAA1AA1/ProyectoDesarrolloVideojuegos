using NUnit.Framework;
using JuegoMental.Core;

public class CortisolModelTests
{
    [Test]
    public void StartsAtZero_NotLost()
    {
        var m = new CortisolModel(100f);
        Assert.AreEqual(0f, m.Value);
        Assert.IsFalse(m.IsLost);
    }

    [Test]
    public void Add_RaisesValue()
    {
        var m = new CortisolModel(100f);
        m.Add(30f);
        Assert.AreEqual(30f, m.Value);
    }

    [Test]
    public void Add_ClampsAtMax_AndMarksLost()
    {
        var m = new CortisolModel(100f);
        m.Add(150f);
        Assert.AreEqual(100f, m.Value);
        Assert.IsTrue(m.IsLost);
    }

    [Test]
    public void Add_NegativeLowersValue_ClampsAtZero()
    {
        var m = new CortisolModel(100f);
        m.Add(40f);
        m.Add(-100f);
        Assert.AreEqual(0f, m.Value);
        Assert.IsFalse(m.IsLost);
    }
}
