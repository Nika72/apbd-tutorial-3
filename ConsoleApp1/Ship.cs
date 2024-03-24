using System;
using System.Collections.Generic;
using System.Linq;
namespace ConsoleApp1;

public class Ship
{
    public string Name { get; set; }
    public int MaxSpeed { get; set; }
    public int MaxContainerNum { get; set; }
    public double MaxWeight { get; set; }
    public List<Container> Containers { get; } = new List<Container>();

    public Ship(string name, int maxSpeed, int maxContainerNum, double maxWeight)
    {
        Name = name;
        MaxSpeed = maxSpeed;
        MaxContainerNum = maxContainerNum;
        MaxWeight = maxWeight;
    }

    public void LoadContainer(Container container)
    {
        if (Containers.Count >= MaxContainerNum)
            throw new Exception("Ship is at maximum capacity.");
        double totalWeight = Containers.Sum(c => c.CurrentCargoMass + c.TareWeight);
        if (totalWeight + container.MassCapacity > MaxWeight)
            throw new Exception("Ship is overloaded.");

        Containers.Add(container);
    }

    public void UnloadContainer(Container container)
    {
        Containers.Remove(container);
    }
}
