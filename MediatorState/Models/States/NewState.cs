using MediatorState.Models.Interfaces;

namespace MediatorState.Models.States
{
    public class NewState : IDocumentState
    {
        public void Print(Document document)
        {
            document.SetState(new PrintingState());
            document.Mediator.Notify(document, "RequestPrint", document);
        }

        public void AddToQueue(Document document) =>
            document.Mediator.Notify(document, "AddToQueue", document);

        public void CompletePrinting(Document document) =>
            Console.WriteLine("[FSM:New] Документ еще не печатается.");
        public void FailPrinting(Document document) =>
            Console.WriteLine("[FSM:New] Ошибка невозможна для нового документа.");
        public void ResetPrinting(Document document) =>
            Console.WriteLine("[FSM:New] Документ уже в начальном состоянии.");
    }
}