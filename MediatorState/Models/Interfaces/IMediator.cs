using MediatorState.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models.Interfaces
{
    public interface IMediator
    {
        void Notify(Colleague colleague, string ev, Document? document = null);
    }
}
