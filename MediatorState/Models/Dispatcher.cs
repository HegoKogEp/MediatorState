using MediatorState.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models
{
    public class Dispatcher : Colleague
    {
        public void AddToQueue(Document document) => 
            _mediator.Notify(this, "AddToQueue", document);
        public void ProcessQueue() =>
            _mediator.Notify(this, "ProcessQueue");
    }
}
