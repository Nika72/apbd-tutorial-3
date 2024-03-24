using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp1;

public interface IHazardNotifier
{
    void NotifyHazard(string containerNumber);
}

public class OverfillException : Exception
{
    public OverfillException(string message) : base(message) { }
}

public class Cargo
{
    public string Type { get; set; }
    public double Mass { get; set; }
}

public class Container
{
    public string SerialNumber { get; }
    public double MassCapacity { get; }
    public double TareWeight { get; }
    public double CurrentCargoMass { get; protected set; }

    public Container(string serialNumber, double massCapacity, double tareWeight)
    {
        SerialNumber = serialNumber;
        MassCapacity = massCapacity;
        TareWeight = tareWeight;
    }

    public virtual void LoadCargo(Cargo cargo)
    {
        if (cargo.Mass > MassCapacity)
            throw new OverfillException("Cargo mass exceeds container capacity.");

        CurrentCargoMass = cargo.Mass;
    }

    public virtual void EmptyCargo()
    {
        CurrentCargoMass = 0;
    }
}

public class LiquidContainer : Container, IHazardNotifier
{
    public double Pressure { get; }

    public LiquidContainer(string serialNumber, double massCapacity, double tareWeight, double pressure)
        : base(serialNumber, massCapacity, tareWeight)
    {
        Pressure = pressure;
    }

    public override void LoadCargo(Cargo cargo)
    {
        if (cargo.Type == "hazardous")
        {
            if (cargo.Mass > MassCapacity * 0.5)
                throw new OverfillException("Liquid container can only be filled up to 50% of its capacity.");
        }
        else
        {
            // Fill the container to 90% capacity if cargo is not hazardous
            double maximumAllowedMass = MassCapacity * 0.9;
            if (cargo.Mass > maximumAllowedMass)
            {
                Console.WriteLine($"Warning: Liquid container {SerialNumber} is being filled more than 90% of its capacity.");
                CurrentCargoMass = maximumAllowedMass;
            }
            else
            {
                CurrentCargoMass = cargo.Mass;
            }
        }
    }



    public void NotifyHazard(string containerNumber)
    {
        Console.WriteLine($"Hazardous situation detected in liquid container {containerNumber}");
    }
}

public class GasContainer : Container, IHazardNotifier
{
    public double Pressure { get; }

    public GasContainer(string serialNumber, double massCapacity, double tareWeight, double pressure)
        : base(serialNumber, massCapacity, tareWeight)
    {
        Pressure = pressure;
    }

    public override void LoadCargo(Cargo cargo)
    {
        if (cargo.Mass > MassCapacity)
            throw new OverfillException("Cargo mass exceeds container capacity.");

        CurrentCargoMass = cargo.Mass;
    }

    public override void EmptyCargo()
    {
        CurrentCargoMass *= 0.95; // Leave 5% inside
    }

    public void NotifyHazard(string containerNumber)
    {
        Console.WriteLine($"Hazardous situation detected in gas container {containerNumber}");
    }
}

public class RefrigeratedContainer : Container
{
    public Dictionary<string, double> TemperatureMap { get; } = new Dictionary<string, double>()
    {
        { "bananas", 13.3 },
        { "chocolate", 18 },
        { "fish", 2 },
        { "meat", -15 },
        { "ice cream", -18 },
        { "frozen pizza", -30 },
        { "cheese", 7.2 },
        { "sausage", 5 },
        { "butter", 20.5 },
        { "egg", 19 }
    };

    public RefrigeratedContainer(string serialNumber, double massCapacity, double tareWeight)
        : base(serialNumber, massCapacity, tareWeight)
    {
    }

    public override void LoadCargo(Cargo cargo)
    {
        if (!TemperatureMap.ContainsKey(cargo.Type))
            throw new ArgumentException($"Invalid cargo type: {cargo.Type}");

        double requiredTemperature = TemperatureMap[cargo.Type];
        // Check if container temperature is suitable for the cargo
        if (requiredTemperature < -30 || requiredTemperature > 30)
            throw new Exception("Temperature out of range for cargo.");

        CurrentCargoMass = cargo.Mass;
    }

    public override void EmptyCargo()
    {
        CurrentCargoMass = 0;
    }
}

public class UserInterface
{
    private ContainerManagementSystem containerManagementSystem;
    private DataPersistence dataPersistence;
    private UnitTests unitTests;

    public UserInterface(ContainerManagementSystem cms, DataPersistence dp, UnitTests ut)
    {
        containerManagementSystem = cms;
        dataPersistence = dp;
        unitTests = ut;
    }

    public void LoadContainerToShip(Container container, Ship ship)
    {
        try
        {
            // Check if the container with the same serial number already exists on the ship
            if (ship.Containers.Any(c => c.SerialNumber == container.SerialNumber))
            {
                Console.WriteLine($"Error: Container with serial number {container.SerialNumber} already exists on the ship.");
                return;
            }

            containerManagementSystem.LoadContainerToShip(container, ship);
            Console.WriteLine($"Container {container.SerialNumber} loaded onto ship {ship.Name} successfully.");
        }
        catch (ShipCapacityExceededException ex)
        {
            Console.WriteLine($"Error: {ex.Message}. Ship cannot accommodate more containers.");
        }
        catch (ContainerOverfillException ex)
        {
            Console.WriteLine($"Error: {ex.Message}. Container {container.SerialNumber} cannot be overfilled.");
        }
        catch (OverfillException ex)
        {
            Console.WriteLine($"Error: {ex.Message}. Cargo cannot be loaded into the container.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }


    public void UnloadContainerFromShip(Container container, Ship ship)
    {
        try
        {
            containerManagementSystem.UnloadContainerFromShip(container, ship);
            Console.WriteLine($"Container {container.SerialNumber} unloaded from ship {ship.Name} successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void SaveData(string data)
    {
        try
        {
            dataPersistence.SaveData(data);
            Console.WriteLine("Data saved successfully.");
        }
        catch (DataStorageException ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Exception for when the ship's capacity is exceeded
    public class ShipCapacityExceededException : Exception
    {
        public ShipCapacityExceededException(string message) : base(message) { }
    }

    // Exception for when a container is overfilled
    public class ContainerOverfillException : Exception
    {
        public ContainerOverfillException(string message) : base(message) { }
    }

    // Exception for data storage errors
    public class DataStorageException : Exception
    {
        public DataStorageException(string message) : base(message) { }
    }

    // Exception for data retrieval errors
    public class DataRetrievalException : Exception
    {
        public DataRetrievalException(string message) : base(message) { }
    }

    // Exception for unit testing errors
    public class UnitTestingException : Exception
    {
        public UnitTestingException(string message) : base(message) { }
    }

    public string RetrieveData()
    {
        try
        {
            return dataPersistence.RetrieveData();

        }
        catch (DataRetrievalException ex)
        {
            Console.WriteLine($"Error retrieving data: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    public void RunUnitTests()
    {
        try
        {
            unitTests.RunTests();
            Console.WriteLine("Unit tests completed successfully.");
        }
        catch (UnitTestingException ex)
        {
            Console.WriteLine($"Error running unit tests: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

   
}



public class DataPersistence
{
   

    public void SaveData(string data)
    {
       
        Console.WriteLine($"Data saved: {data}");
    }

    public string RetrieveData()
    {
        
        return "Placeholder data";
    }
}




class Program
{
    static void Main(string[] args)
    {
  
        ContainerManagementSystem cms = new ContainerManagementSystem();

        
        DataPersistence dp = new DataPersistence();

        
        UnitTests ut = new UnitTests();

      
        UserInterface ui = new UserInterface(cms, dp, ut);

      
        Container liquidContainer = new LiquidContainer("L123", 1000, 200, 5);
        Container gasContainer = new GasContainer("G456", 800, 150, 3);
        Container refrigeratedContainer = new RefrigeratedContainer("R789", 1200, 300);

        
        Ship ship = new Ship("Ship1", 50, 5, 5000);

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Load container onto ship");
            Console.WriteLine("2. Unload container from ship");
            Console.WriteLine("3. Save data");
            Console.WriteLine("4. Retrieve data");
            Console.WriteLine("5. Run unit tests");
            Console.WriteLine("6. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Enter container type (liquid/gas/refrigerated):");
                    string containerType = Console.ReadLine().ToLower(); // Convert to lowercase for consistency
                    Console.WriteLine("Enter container serial number:");
                    string containerSerialNumber = Console.ReadLine();
                    Console.WriteLine("Enter container mass capacity:");
                    double containerMassCapacity = double.Parse(Console.ReadLine());
                    Console.WriteLine("Enter container tare weight:");
                    double containerTareWeight = double.Parse(Console.ReadLine());

                    Container container;
                    switch (containerType)
                    {
                        case "liquid":
                            Console.WriteLine("Enter container pressure:");
                            double containerPressure = double.Parse(Console.ReadLine());
                            container = new LiquidContainer(containerSerialNumber, containerMassCapacity, containerTareWeight, containerPressure);
                            break;
                        case "gas":
                            Console.WriteLine("Enter container pressure:");
                            containerPressure = double.Parse(Console.ReadLine());
                            container = new GasContainer(containerSerialNumber, containerMassCapacity, containerTareWeight, containerPressure);
                            break;
                        case "refrigerated":
                            container = new RefrigeratedContainer(containerSerialNumber, containerMassCapacity, containerTareWeight);
                            break;
                        default:
                            Console.WriteLine("Invalid container type.");
                            continue;
                    }

                    // Add prompts for cargo type and mass
                    Console.WriteLine("Enter cargo type:");
                    string cargoType = Console.ReadLine();
                    Console.WriteLine("Enter cargo mass:");
                    double cargoMass = double.Parse(Console.ReadLine());
                    Cargo cargo = new Cargo() { Type = cargoType, Mass = cargoMass };

                    ui.LoadContainerToShip(container, ship);
                    break;


                case "2":
                   
                    Console.WriteLine("Enter container serial number to unload:");
                    string containerToUnload = Console.ReadLine();
                    Container containerToRemove = ship.Containers.FirstOrDefault(c => c.SerialNumber == containerToUnload);
                    if (containerToRemove != null)
                    {
                        ui.UnloadContainerFromShip(containerToRemove, ship);
                    }
                    else
                    {
                        Console.WriteLine("Container not found on the ship.");
                    }
                    break;

                case "3":
                    Console.WriteLine("Enter data to save:");
                    string data = Console.ReadLine();
                    ui.SaveData(data);
                    break;

                case "4":
                    string retrievedData = ui.RetrieveData();
                    Console.WriteLine($"Retrieved data: {retrievedData}");
                    break;

                case "5":
                    ui.RunUnitTests();
                    break;

                case "6":
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Invalid option. Please choose again.");
                    break;
            }
        }
    }
}
