using MediatorState.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models
{
    public class Printer : Colleague
    {
        public bool Failure { get; set; } = false;

        public void StartPrint(Document document)
        {
            Console.WriteLine($"Печать документа: {document.Title}");
            if (Failure)
            {
                Failure = false;
                _mediator.Notify(this, "PrintFailed", document);
            }
            else
            {
                _mediator.Notify(this, "PrintSuccess", document);
            }
        }
    }
}
