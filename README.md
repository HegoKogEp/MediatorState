# MediatorState - Документация по проекту

## Содержание
1. [Mediatorstate](#mediatorstate)
   1. [MediatorState.csproj](#mediatorstatecsproj)
   2. [Program.cs](#programcs)
2. [Mediatorstate/Models](#mediatorstate-models)
   1. [Document.cs](#documentcs)
3. [Mediatorstate/Models/Base](#mediatorstate-models-base)
   1. [Colleague.cs](#colleaguecs)
4. [Mediatorstate/Models/Interfaces](#mediatorstate-models-interfaces)
   1. [IDocumentState.cs](#idocumentstatecs)
   2. [IMediator.cs](#imediatorcs)
5. [Mediatorstate/Models/States](#mediatorstate-models-states)
   1. [DoneState.cs](#donestatecs)
   2. [ErrorState.cs](#errorstatecs)
   3. [NewState.cs](#newstatecs)
   4. [PrintingState.cs](#printingstatecs)

## FILE 1: MediatorState.csproj

<a id='mediatorstatecsproj'></a>

```xml
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

---

## FILE 2: Colleague.cs

<a id='colleaguecs'></a>

```csharp
﻿using MediatorState.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models.Base
{
    public abstract class Colleague
    {
        protected IMediator _mediator;

        public void SetMediator(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
```

---

## FILE 3: Document.cs

<a id='documentcs'></a>

```csharp
﻿using MediatorState.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models
{
    public class Document
    {
        private IDocumentState _state;

        public void SetState(IDocumentState documentState) => _state = documentState;

        public void Print() => _state.Print(this);

        public void AddToQueue() => _state.AddToQueue(this);

        public void CompletePrinting() => _state.CompletePrinting(this);

        public void FailPrinting() => _state.FailPrinting(this);

        public void Reset() => _state.ResetPrinting(this);
    }
}
```

---

## FILE 4: IDocumentState.cs

<a id='idocumentstatecs'></a>

```csharp
﻿using System;
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
```

---

## FILE 5: IMediator.cs

<a id='imediatorcs'></a>

```csharp
﻿using MediatorState.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models.Interfaces
{
    public interface IMediator
    {
        void Notify(Colleague colleague);
    }
}
```

---

## FILE 6: DoneState.cs

<a id='donestatecs'></a>

```csharp
﻿using MediatorState.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models.States
{
    public class DoneState : IDocumentState
    {
    }
}
```

---

## FILE 7: ErrorState.cs

<a id='errorstatecs'></a>

```csharp
﻿using MediatorState.Models.Interfaces;
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
```

---

## FILE 8: NewState.cs

<a id='newstatecs'></a>

```csharp
﻿using MediatorState.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models.States
{
    public class NewState : IDocumentState
    {
        public void AddToQueue(Document document)
        {
            throw new NotImplementedException();
        }

        public void CompletePrinting(Document document)
        {
            throw new NotImplementedException();
        }

        public void FailPrinting(Document document)
        {
            throw new NotImplementedException();
        }

        public void Print(Document document)
        {
            throw new NotImplementedException();
        }

        public void ResetPrinting(Document document)
        {
            throw new NotImplementedException();
        }
    }
}
```

---

## FILE 9: PrintingState.cs

<a id='printingstatecs'></a>

```csharp
﻿using MediatorState.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models.States
{
    public class PrintingState : IDocumentState
    {
    }
}
```

---

## FILE 10: Program.cs

<a id='programcs'></a>

```csharp
﻿namespace MediatorState
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
        }
    }
}
```

---

