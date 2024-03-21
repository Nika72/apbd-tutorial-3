namespace ConsoleApp1;

public class UnitTests
{
    public void RunTests()
    {
        Console.WriteLine("Running unit tests...");

        TestLoadingCargo();
    }

    private void TestLoadingCargo()
    {
        Container liquidContainer = new LiquidContainer("L123", 1000, 200, 5);
        Container gasContainer = new GasContainer("G456", 800, 150, 3);
        Container refrigeratedContainer = new RefrigeratedContainer("R789", 1200, 300);

        Cargo hazardousCargo = new Cargo() { Type = "hazardous", Mass = 600 };
        Cargo nonHazardousCargo = new Cargo() { Type = "non-hazardous", Mass = 500 };

        try
        {
            liquidContainer.LoadCargo(hazardousCargo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading hazardous cargo into liquid container: {ex.Message}");
        }

        try
        {
            gasContainer.LoadCargo(nonHazardousCargo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading non-hazardous cargo into gas container: {ex.Message}");
        }

        try
        {
            refrigeratedContainer.LoadCargo(nonHazardousCargo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading cargo into refrigerated container: {ex.Message}");
        }
    }
}