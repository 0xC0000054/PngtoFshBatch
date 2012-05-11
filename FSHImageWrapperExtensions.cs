using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using FshDatIO;

namespace PngtoFshBatchtxt
{
    static class FSHImageWrapperExtensions
    {
        private static FieldInfo rawDataField;
        public static void SetRawData(this FSHImageWrapper fsh, byte[] data)
        {
            if (rawDataField == null)
	        {
		        Type fshImageType = typeof(FSHImageWrapper);
                rawDataField = fshImageType.GetField("rawData", BindingFlags.Instance | BindingFlags.NonPublic);
	        }
            rawDataField.SetValue(fsh, data);
        }
    }
}
