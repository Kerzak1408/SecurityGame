using System.Text;
using UnityEngine;

namespace Assets.Scripts.DataStructures
{
    /// <summary>
    /// Multi-dimensional cost. Higher index == Lower priority. By comparison, first values are compared and only when they 
    /// are equal the second ones are compared etc.
    /// </summary>
    public class PriorityCost
    {
        private readonly float[] costs;

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

        /// <summary>
        /// Sums values of each index, respectively.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static PriorityCost operator + (PriorityCost first, PriorityCost second)
        {
            var result = new PriorityCost(first.Length);
            for (int i = 0; i < first.Length; i++)
            {
                result[i] = first[i] + second[i];
            }
            return result;
        }

        /// <summary>
        /// Compares the first components, in case of equality, second components, in case
        /// od equality third components etc.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator < (PriorityCost left, PriorityCost right)
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

        /// <summary>
        /// Compares the first components, in case of equality, second components, in case
        /// od equality third components etc.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator > (PriorityCost left, PriorityCost right)
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
            foreach (var cost in costs)
            {
                stringBuilder.Append(cost).Append(",");
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Round each component to the <paramref name="decimalPlaces"/>.
        /// </summary>
        /// <param name="decimalPlaces"></param>
        public void Round(int decimalPlaces)
        {
            float pow = Mathf.Pow(10, decimalPlaces);
            for (int i = 0; i < costs.Length; i++)
            {
                costs[i] = Mathf.Round(costs[i] * pow) / pow;
            }
        }
    }
}
