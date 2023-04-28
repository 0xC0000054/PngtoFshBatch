/*
* This file is part of PngtoFshBatch, a tool for batch converting images
* to FSH.
*
* Copyright (C) 2009-2017, 2023 Nicholas Hayes
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*
*/

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

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
