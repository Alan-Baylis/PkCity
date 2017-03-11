using System.Collections.Generic;
using System.Linq;

namespace Pk.Functions {
    public class FrequencyList<T> : List<T> {
        public FrequencyList(T[] list, int[] frequency) {
            int i;
            for (i = 0; i < frequency.Length && i < list.Length; i++)
                for (int j = 0; j < frequency[i]; j++)
                    Add(list[i]);
        }
	}
}
