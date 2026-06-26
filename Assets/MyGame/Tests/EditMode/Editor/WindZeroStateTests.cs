using NUnit.Framework;

public class WindZeroStateTests
{
    [Test]
    public void SliderMinimumProducesZeroWind()
    {
        float windSpeed = WindSimulationController.CalculateWindSpeedFromSlider(0f, 255f, 0f, 30f);

        Assert.AreEqual(0f, windSpeed);
    }

    [Test]
    public void ZeroWindDisablesRotorAnimation()
    {
        Assert.IsFalse(WindSimulationController.ShouldAnimateAtWindSpeed(0f));
        Assert.AreEqual(0f, WindSimulationController.CalculateRotorStep(0f, 720f, 0.016f));
        Assert.AreEqual(0f, WindSimulationController.CalculateRotorRpm(0f, 720f));
    }

    [Test]
    public void ZeroWindForcesDashboardEfficiencyToZero()
    {
        float efficiency = WindradDashboard.CalculateTurbineEfficiency(1f, 0.59f, 0f);

        Assert.AreEqual(0f, efficiency);
    }

    [Test]
    public void RotorRpmIsProportionalToWindSpeed()
    {
        float rpm = WindSimulationController.CalculateRotorRpm(0.5f, 720f);

        Assert.AreEqual(60f, rpm);
    }
}
