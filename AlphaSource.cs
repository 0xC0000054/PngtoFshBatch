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

namespace PngtoFshBatchtxt
{
    internal enum AlphaSource
    {
        /// <summary>
        /// The alpha map is from a separate file.
        /// </summary>
        File,
        /// <summary>
        /// The alpha map is from the image transparency.
        /// </summary>
        Transparency,
        /// <summary>
        /// The alpha map is generated for an image without transparency.
        /// </summary>
        Generated
    }
}