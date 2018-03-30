using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DataStructures
{
    public class IntegerTuple : Tuple<int, int>
    {
        public IntegerTuple(int first, int second) : base(first, second)
        {
        }

        public static IntegerTuple operator + (IntegerTuple tuple1, IntegerTuple tuple2)
        {
            return new IntegerTuple(tuple1.First + tuple2.First, tuple1.Second + tuple2.Second);
        }

        public static IntegerTuple operator *(IntegerTuple tuple1, int multiplier)
        {
            return new IntegerTuple(tuple1.First * multiplier, tuple1.Second * multiplier);
        }

        public static implicit operator Vector3(IntegerTuple tuple)
        {
            return new Vector3(tuple.First, tuple.Second);
        }
    }
}
