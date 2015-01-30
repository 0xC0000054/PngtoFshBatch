using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace PngtoFshBatchtxt
{
    internal sealed class BatchFshCollection : Collection<BatchFshContainer>, IDisposable
    {
        private bool disposed;

        public BatchFshCollection() : this(0)
        {
        }
        
        public BatchFshCollection(int count) : base(new List<BatchFshContainer>(count)) 
        {
            this.disposed = false;
        }


        protected override void RemoveItem(int index)
        {
            BatchFshContainer item = Items[index];
            if (item != null)
            {
                item.Dispose();
            }

            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            IList<BatchFshContainer> items = Items;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    items[i].Dispose();
                }
            }

            base.ClearItems();
        }

        public void SetCapacity(int newCapacity)
        {
            var list = (List<BatchFshContainer>)Items;

            if (list.Capacity < newCapacity)
            {
                list.Capacity = newCapacity;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                this.disposed = true;

                if (disposing)
                {
                    var items = Items;

                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i] != null)
                        {
                            items[i].Dispose();
                        }
                    }
                }                
            }
        }
    }
}
