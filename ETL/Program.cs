using ETL.Service.Implementations;
using ETL.Service.Interfaces;

class Program
{
    static async Task Main(string[] args)
    {
        IUI ui = new UI();
        await ui.Menu();
    }
}