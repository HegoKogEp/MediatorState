# Project Documentation

Generated on: 2026-04-09 21:51:44

## Table of Contents

1. [MediatorState.csproj](#mediatorstate-csproj)
2. [Program.cs](#program-cs)
3. [Models/Dispatcher.cs](#models-dispatcher-cs)
4. [Models/Document.cs](#models-document-cs)
5. [Models/Logger.cs](#models-logger-cs)
6. [Models/Printer.cs](#models-printer-cs)
7. [Models/PrintQueue.cs](#models-printqueue-cs)
8. [Models/PrintSystemMediator.cs](#models-printsystemmediator-cs)
9. [Models/Base/Colleague.cs](#models-base-colleague-cs)
10. [Models/Interfaces/IDocumentState.cs](#models-interfaces-idocumentstate-cs)
11. [Models/Interfaces/IMediator.cs](#models-interfaces-imediator-cs)
12. [Models/States/DoneState.cs](#models-states-donestate-cs)
13. [Models/States/ErrorState.cs](#models-states-errorstate-cs)
14. [Models/States/NewState.cs](#models-states-newstate-cs)
15. [Models/States/PrintingState.cs](#models-states-printingstate-cs)
16. [obj/Debug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs](#obj-debug-net10-0-netcoreapp,version=v10-0-assemblyattributes-cs)
17. [obj/Debug/net10.0/MediatorState.AssemblyInfo.cs](#obj-debug-net10-0-mediatorstate-assemblyinfo-cs)
18. [obj/Debug/net10.0/MediatorState.GlobalUsings.g.cs](#obj-debug-net10-0-mediatorstate-globalusings-g-cs)

---

## MediatorState.csproj

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\MediatorState.csproj`

**File type:** `.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>

```

---

## Program.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Program.cs`

**File type:** `.cs`

```csharp
using MediatorState.Models;

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

## Models/Dispatcher.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\Dispatcher.cs`

**File type:** `.cs`

```csharp
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

```

---

## Models/Document.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\Document.cs`

**File type:** `.cs`

```csharp
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

```

---

## Models/Logger.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\Logger.cs`

**File type:** `.cs`

```csharp
using MediatorState.Models.Base;
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

## Models/Printer.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\Printer.cs`

**File type:** `.cs`

```csharp
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

```

---

## Models/PrintQueue.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\PrintQueue.cs`

**File type:** `.cs`

```csharp
using MediatorState.Models.Base;
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

## Models/PrintSystemMediator.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\PrintSystemMediator.cs`

**File type:** `.cs`

```csharp
using MediatorState.Models.Base;
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

## Models/Base/Colleague.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\Base\Colleague.cs`

**File type:** `.cs`

```csharp
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

```

---

## Models/Interfaces/IDocumentState.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\Interfaces\IDocumentState.cs`

**File type:** `.cs`

```csharp
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

```

---

## Models/Interfaces/IMediator.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\Interfaces\IMediator.cs`

**File type:** `.cs`

```csharp
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

```

---

## Models/States/DoneState.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\States\DoneState.cs`

**File type:** `.cs`

```csharp
using MediatorState.Models.Interfaces;
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

## Models/States/ErrorState.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\States\ErrorState.cs`

**File type:** `.cs`

```csharp
using MediatorState.Models.Interfaces;
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

## Models/States/NewState.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\States\NewState.cs`

**File type:** `.cs`

```csharp
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
```

---

## Models/States/PrintingState.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\Models\States\PrintingState.cs`

**File type:** `.cs`

```csharp
using MediatorState.Models.Interfaces;
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

## obj/Debug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\obj\Debug\net10.0\.NETCoreApp,Version=v10.0.AssemblyAttributes.cs`

**File type:** `.cs`

```csharp
// <autogenerated />
using System;
using System.Reflection;
[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(".NETCoreApp,Version=v10.0", FrameworkDisplayName = ".NET 10.0")]

```

---

## obj/Debug/net10.0/MediatorState.AssemblyInfo.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\obj\Debug\net10.0\MediatorState.AssemblyInfo.cs`

**File type:** `.cs`

```csharp
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;

[assembly: System.Reflection.AssemblyCompanyAttribute("MediatorState")]
[assembly: System.Reflection.AssemblyConfigurationAttribute("Debug")]
[assembly: System.Reflection.AssemblyFileVersionAttribute("1.0.0.0")]
[assembly: System.Reflection.AssemblyInformationalVersionAttribute("1.0.0+f920f86805c364b7de33e4e5c2df34a3344ab05f")]
[assembly: System.Reflection.AssemblyProductAttribute("MediatorState")]
[assembly: System.Reflection.AssemblyTitleAttribute("MediatorState")]
[assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]

// Создано классом WriteCodeFragment MSBuild.


```

---

## obj/Debug/net10.0/MediatorState.GlobalUsings.g.cs

**File path:** `D:\ResoursesVisualStudio\MediatorState\MediatorState\obj\Debug\net10.0\MediatorState.GlobalUsings.g.cs`

**File type:** `.cs`

```csharp
// <auto-generated/>
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;

```

---

