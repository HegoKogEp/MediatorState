using MediatorState.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models.States
{
    public class DoneState : IDocumentState
    {
        public void Print(Document document) => 
            Console.WriteLine("[FSM:Done] Документ уже напечатан.");
        public void AddToQueue(Document document) => 
            Console.WriteLine("[FSM:Done] Документ уже напечатан, не может быть добавлен в очередь.");
        public void CompletePrinting(Document document) => 
            Console.WriteLine("[FSM:Done] Документ уже завершен.");
        public void FailPrinting(Document document) => 
            Console.WriteLine("[FSM:Done] Не удалось вызвать ошибку для завершенного документа.");
        public void ResetPrinting(Document document) => 
            Console.WriteLine("[FSM:Done] Документ в финальном состоянии, сброс невозможен.");
    }
}
