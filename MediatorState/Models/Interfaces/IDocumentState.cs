using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models.Interfaces
{
    public interface IDocumentState
    {
        void Print(Document document);
        void AddToQueue(Document document);
        void CompletePrinting(Document document);
        void FailPrinting(Document document);
        void ResetPrinting(Document document);
    }
}
