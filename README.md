# MediatorState - Документация по проекту

## Содержание
1. [Mediatorstate](#mediatorstate)
   1. [MediatorState.csproj](#mediatorstatecsproj)
   2. [Program.cs](#programcs)
2. [Mediatorstate/Models](#mediatorstate-models)
   1. [Dispatcher.cs](#dispatchercs)
   2. [Document.cs](#documentcs)
   3. [Logger.cs](#loggercs)
   4. [PrintQueue.cs](#printqueuecs)
   5. [PrintSystemMediator.cs](#printsystemmediatorcs)
   6. [Printer.cs](#printercs)
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
        protected IMediator? _mediator;
        public IMediator Mediator { get => _mediator; }

        public void SetMediator(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
```

---

## FILE 3: Dispatcher.cs

<a id='dispatchercs'></a>

```csharp
﻿using MediatorState.Models.Base;
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
```

---

## FILE 4: Document.cs

<a id='documentcs'></a>

```csharp
﻿using MediatorState.Models.Base;
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
```

---

## FILE 5: IDocumentState.cs

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

## FILE 6: IMediator.cs

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
        void Notify(Colleague colleague, string ev, Document? document = null);
    }
}
```

---

## FILE 7: Logger.cs

<a id='loggercs'></a>

```csharp
﻿using MediatorState.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models
{
    public class Logger : Colleague
    {
        public void WriteMessage(string message) =>
            Console.WriteLine($"[Лог] {message}");
    }
}
```

---

## FILE 8: PrintQueue.cs

<a id='printqueuecs'></a>

```csharp
﻿using MediatorState.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models
{
    public class PrintQueue : Colleague
    {
        private readonly Queue<Document> _queue = new();

        public void EnqueueItem(Document item)
        {
            _queue.Enqueue(item);
            _mediator.Notify(this, "Enqueued", item);
        }

        public Document? DequeueItem()
        {
            if (_queue.Count > 0)
            {
                return _queue.Dequeue();
            }
            else
            {
                return null;
            }
        }

        public bool IsEmpty => _queue.Count == 0;
    }
}
```

---

## FILE 9: PrintSystemMediator.cs

<a id='printsystemmediatorcs'></a>

```csharp
﻿using MediatorState.Models.Base;
using MediatorState.Models.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models
{
    public class PrintSystemMediator : IMediator
    {
        private readonly Printer? _printer;
        private readonly PrintQueue? _printQueue;
        private readonly Logger? _logger;

        public PrintSystemMediator(Printer? printer, PrintQueue? printQueue, Logger? logger)
        {
            _printer = printer;
            _printQueue = printQueue;
            _logger = logger;

            _printer.SetMediator(this);
            _printQueue.SetMediator(this);
            _logger.SetMediator(this);
        }

        public void Notify(Colleague colleague, string ev, Document? document = null)
        {
            switch (ev)
            {
                case "AddToQueue":
                    _printQueue.EnqueueItem(document);
                    break;

                case "Enqueued":
                    _logger.WriteMessage($"Документ '{document.Title}' помещен в очередь");
                    break;

                case "RequestPrint":
                    _printer.StartPrint(document);
                    break;

                case "ProcessQueue":
                    if (_printQueue.IsEmpty)
                    {
                        _logger.WriteMessage("Очередь пуста.");
                        return;
                    }
                    var nextDoc = _printQueue.DequeueItem();
                    nextDoc.SetMediator(this);
                    nextDoc.Print();
                    break;

                case "PrintSuccess":
                    document!.CompletePrinting();
                    _logger.WriteMessage($"Успешно напечатан '{document.Title}'.");
                    break;

                case "PrintFailed":
                    document!.FailPrinting();
                    _logger.WriteMessage($"Ошибка печати '{document.Title}'.");
                    break;
            }
        }
    }
}
```

---

## FILE 10: Printer.cs

<a id='printercs'></a>

```csharp
﻿using MediatorState.Models.Base;
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
```

---

## FILE 11: DoneState.cs

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
```

---

## FILE 12: ErrorState.cs

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

## FILE 13: NewState.cs

<a id='newstatecs'></a>

```csharp
﻿using MediatorState.Models.Interfaces;

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
```

---

## FILE 14: PrintingState.cs

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
```

---

## FILE 15: Program.cs

<a id='programcs'></a>

```csharp
﻿using MediatorState.Models;

namespace MediatorState
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Система управления печатью документов с помощью паттерна 'Состояние' и 'Посредник'.");

            var printer = new Printer();
            var queue = new PrintQueue();
            var logger = new Logger();
            var mediator = new PrintSystemMediator(printer, queue, logger);
            var dispatcher = new Dispatcher();
            dispatcher.SetMediator(mediator);

            Console.WriteLine("--------Успешная печать--------");
            Document document1 = new Document("Документ 1");
            Document document2 = new Document("Документ 2");
            document1.SetMediator(mediator);
            document2.SetMediator(mediator);

            dispatcher.AddToQueue(document1);
            dispatcher.AddToQueue(document2);
            dispatcher.ProcessQueue();
            dispatcher.ProcessQueue();

            Console.WriteLine("--------Ошибка печати--------");
            Document document3 = new Document("Документ 3");
            document3.SetMediator(mediator);

            dispatcher.AddToQueue(document3);
            printer.Failure = true; // Симулируем ошибку печати
            dispatcher.ProcessQueue();

            document3.Reset(); // Сброс состояния документа
            dispatcher.AddToQueue(document3);
            dispatcher.ProcessQueue();

            Console.WriteLine("--------Финальное состояние--------");
            document1.Print();
            document1.AddToQueue();

        }
    }
}
```

---

