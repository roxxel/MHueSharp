using MagicHue;

var bulbs = Discover.Start();
if (bulbs.Count == 0)
{
    while (bulbs.Count == 0)
        bulbs = Discover.Start();
}

var light = bulbs.First();
Console.WriteLine($"{light.Address} {light.Name}");
light.Connect();

while (true)
{
    try
    {
        var values = Console.ReadLine().Split(" ").Select(x => Convert.ToByte(x)).ToArray();
        light.RGB = (values[0], values[1], values[2]);
    }
    catch (Exception)
    {

    }
}

