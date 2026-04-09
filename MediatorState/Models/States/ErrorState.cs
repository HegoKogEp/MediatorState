using MediatorState.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MediatorState.Models.States
{
    public class ErrorState : IDocumentState
    {
        public void Print(Document document) =>
            Console.WriteLine("[FSM:Error] Печать невозможна из-за ошибки. Сначала сбросьте документ (Reset)");
        public void AddToQueue(Document document) =>
            Console.WriteLine("[FSM: Error] Нельзя добавить в очередь из-за ошибки. Сначала сбросьте документ.");
        public void CompletePrinting(Document document) =>
            Console.WriteLine("[FSM: Error] Ошибка не устранена.");
        public void FailPrinting(Document document) =>
            Console.WriteLine("[FSMError] Документ уже в состоянии ошибки.");

        public void ResetPrinting(Document document)
        {
            document.SetState(new NewState());
            Console.WriteLine("[FSM: Error -> New] Документ сброшен и готов к повторной печати.");
        }
    }
}
