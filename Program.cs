using OpenTK.Windowing.Desktop;

public static class Program
{
    private static void Main()
    {
        using Window window = new Window(1280, 800);
        window.Run();
    }
}