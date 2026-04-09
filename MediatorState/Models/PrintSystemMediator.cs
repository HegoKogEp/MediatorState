using MediatorState.Models.Base;
using MediatorState.Models.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models
{
    public class PrintSystemMediator : IMediator
    {
        private readonly Printer? _printer;
        private readonly PrintQueue? _printQueue;
        private readonly Logger? _logger;

        public PrintSystemMediator(Printer? printer, PrintQueue? printQueue, Logger? logger)
        {
            _printer = printer;
            _printQueue = printQueue;
            _logger = logger;

            _printer.SetMediator(this);
            _printQueue.SetMediator(this);
            _logger.SetMediator(this);
        }

        public void Notify(Colleague colleague, string ev, Document? document = null)
        {
            switch (ev)
            {
                case "AddToQueue":
                    _printQueue.EnqueueItem(document);
                    break;

                case "Enqueued":
                    _logger.WriteMessage($"Документ '{document.Title}' помещен в очередь");
                    break;

                case "RequestPrint":
                    _printer.StartPrint(document);
                    break;

                case "ProcessQueue":
                    if (_printQueue.IsEmpty)
                    {
                        _logger.WriteMessage("Очередь пуста.");
                        return;
                    }
                    var nextDoc = _printQueue.DequeueItem();
                    nextDoc.SetMediator(this);
                    nextDoc.Print();
                    break;

                case "PrintSuccess":
                    document!.CompletePrinting();
                    _logger.WriteMessage($"Успешно напечатан '{document.Title}'.");
                    break;

                case "PrintFailed":
                    document!.FailPrinting();
                    _logger.WriteMessage($"Ошибка печати '{document.Title}'.");
                    break;
            }
        }
    }
}
