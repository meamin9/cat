using System.Collections;
using System.Collections.Generic;
using Automata.Base;

namespace Automata.Game
{
    public static class UIID 
    {
        private static int _id = 0;
        public static int Id { get { return ++_id; } }
        public static int Loading = Id;
        public static int Joystick = Id;

    }




}
