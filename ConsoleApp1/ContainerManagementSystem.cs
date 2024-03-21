namespace ConsoleApp1;

public class ContainerManagementSystem
{
    private List<Ship> ships = new List<Ship>();

    public void AddShip(Ship ship)
    {
        ships.Add(ship);
    }

    public void LoadContainerToShip(Container container, Ship ship)
    {
        ship.LoadContainer(container);
    }

    public void UnloadContainerFromShip(Container container, Ship ship)
    {
        ship.UnloadContainer(container);
    }

    
}