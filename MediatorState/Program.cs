using MediatorState.Models;

namespace MediatorState
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Система управления печатью документов с помощью паттерна 'Состояние' и 'Посредник'.");

            var printer = new Printer();
            var queue = new PrintQueue();
            var logger = new Logger();
            var mediator = new PrintSystemMediator(printer, queue, logger);
            var dispatcher = new Dispatcher();
            dispatcher.SetMediator(mediator);

            Console.WriteLine("--------Успешная печать--------");
            Document document1 = new Document("Документ 1");
            Document document2 = new Document("Документ 2");
            document1.SetMediator(mediator);
            document2.SetMediator(mediator);

            dispatcher.AddToQueue(document1);
            dispatcher.AddToQueue(document2);
            dispatcher.ProcessQueue();
            dispatcher.ProcessQueue();

            Console.WriteLine("--------Ошибка печати--------");
            Document document3 = new Document("Документ 3");
            document3.SetMediator(mediator);

            dispatcher.AddToQueue(document3);
            printer.Failure = true; // Симулируем ошибку печати
            dispatcher.ProcessQueue();

            document3.Reset(); // Сброс состояния документа
            dispatcher.AddToQueue(document3);
            dispatcher.ProcessQueue();

            Console.WriteLine("--------Финальное состояние--------");
            document1.Print();
            document1.AddToQueue();

        }
    }
}
