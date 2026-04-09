using MediatorState.Models.Base;
using MediatorState.Models.Interfaces;
using MediatorState.Models.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models
{
    public class Document : Colleague
    {
        private IDocumentState _state;
        public string Title { get; set; }
        public void SetState(IDocumentState documentState) => _state = documentState;

        public Document(string title)
        {
            Title = title;
            SetState(new NewState());
        }

        public void Print() => _state.Print(this);

        public void AddToQueue() => _state.AddToQueue(this);

        public void CompletePrinting() => _state.CompletePrinting(this);

        public void FailPrinting() => _state.FailPrinting(this);

        public void Reset() => _state.ResetPrinting(this);
    }
}
