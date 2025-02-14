﻿namespace ISynergy.Framework.Mathematics.IO.Mat
{
    using System.Runtime.InteropServices;

    /*
          
     0   ||   1   ||   2   ||   3   ||   4   ||   5   ||   6   ||   7    
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ||         data type          ||          number of bytes         |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ||                                                               |
    ~                       variable size                           ~
    ||                                                               |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                 MAT-file data element format (v5)
                 8 bytes header / variable payload
     0   ||   1   ||   2   ||   3   ||   4   ||   5   ||   6   ||   7    
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ||   number of bytes  ||   data type   ||           data           |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                MAT-file small data element format (v5)
                    8 bytes header + fixed payload
         
 */
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 8)]
    internal unsafe struct MatDataTag
    {
        [FieldOffset(0)]
        public MatDataType DataType;

        [FieldOffset(4)]
        public int NumberOfBytes;

        [FieldOffset(2)]
        public short SmallData_NumberOfBytes;

        [FieldOffset(0)]
        public MatDataType SmallData_Type;

        [FieldOffset(4)]
        public fixed byte SmallData_Value[4];

        public bool IsSmallFormat { get { return SmallData_NumberOfBytes != 0; } }

    }
}
