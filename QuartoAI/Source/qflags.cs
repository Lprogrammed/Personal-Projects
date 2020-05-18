using System;
using System.Collections.Generic;
using System.Text;

namespace QuartoAI_v2
{
    class qflags
    {
        public const byte OWNER = 0;    //T:AI,F:Opponent
        public const byte TYPE = 1;     //T:pick,F:place
        public const byte ENDBOARD = 2;
        public const byte CHILDHASLOSINGBOARD = 3;
        public const byte ALLCHILDRENSIMULATED = 4;
        public const byte CHILDHASWINNINGBOARD = 5;
    }
}
