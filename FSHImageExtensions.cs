using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSHLib;
using System.Reflection;

namespace PngtoFshBatchtxt
{
    static class FSHImageExtensions
    {
        private static FieldInfo rawDataField;
        public static void SetRawData(this FSHImage fsh, byte[] data)
        {
            if (rawDataField == null)
	        {
		        Type fshImageType = typeof(FSHImage);
                rawDataField = fshImageType.GetField("rawData", BindingFlags.Instance | BindingFlags.NonPublic);
	        }
            rawDataField.SetValue(fsh, data);
        }
    }
}
