using MediatorState.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models.States
{
    public class NewState : IDocumentState
    {
        public void Print(Document document) =>
             Console.WriteLine("[FSM:Printing] Документ уже в процессе печати.");
        public void AddToQueue(Document document) =>
            Console.WriteLine("[FSM:Printing] Нельзя добавить в очередь документ, который печатается.");

        public void CompletePrinting(Document document)
        {
            document.SetState(new DoneState());
        }

        public void FailPrinting(Document document)
        {
            document.SetState(new ErrorState());
        }

        public void ResetPrinting(Document document) =>
            Console.WriteLine("[FSM:Printing] Нельзя сбросить документ во время печати.");
    }
}
