using MediatorState.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models.Base
{
    public abstract class Colleague
    {
        protected IMediator? _mediator;
        public IMediator Mediator { get => _mediator; }

        public void SetMediator(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
