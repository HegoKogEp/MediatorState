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
