namespace SimpleConsole.Test.Execution
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleMarkupColor.WriteLine("### [red]he[blue]llo[/blue][/red] [blue]w[red]orl[yellow]d[/yellow][/red] :)[/blue] [yellow]!![/yellow]!!");
            ConsoleMarkupColor.WriteLine(@"### [blue]\[\[/blue][red]red[/red][blue]\]\[/blue]");
            Console.ReadKey(); // Keep the console open to see the output
        }
    }
}
