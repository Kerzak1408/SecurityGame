using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DataStructures
{
    public class PriorityCost
    {
        private float[] costs;

        public int Length
        {
            get { return costs.Length; }
        }

        public float this[int i]
        {
            get { return costs[i]; }
            set { costs[i] = value; }
        }

        public PriorityCost(int prioritiesCount)
        {
            costs = new float[prioritiesCount];
        }

        public PriorityCost(int prioritiesCount, float defaultValue)
        {
            costs = new float[prioritiesCount];
            for (int i = 0; i < prioritiesCount; i++)
            {
                costs[i] = defaultValue;
            }
        }

        public void AddCost(int priorityIndex, float cost)
        {
            costs[priorityIndex] = cost;
        }

        public static PriorityCost operator +(PriorityCost first, PriorityCost second)
        {
            var result = new PriorityCost(first.Length);
            for (int i = 0; i < first.Length; i++)
            {
                result[i] = first[i] + second[i];
            }
            return result;
        }

        public static bool operator <(PriorityCost left, PriorityCost right)
        {
            for (int i = 0; i < left.Length; i++)
            {
                float currentLeft = left[i];
                float currentRight = right[i];
                if (currentLeft < currentRight)
                {
                    return true;
                }

                if (currentLeft > currentRight)
                {
                    return false;
                }
            }

            return false;
        }

        public static bool operator >(PriorityCost left, PriorityCost right)
        {
            return right < left;
        }

        public static implicit operator PriorityCost(float value)
        {
            return new PriorityCost(1, value);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            for (int i = 0; i < costs.Length; i++)
            {
                stringBuilder.Append(costs[i]).Append(",");
            }
            return stringBuilder.ToString();
        }
    }
}
