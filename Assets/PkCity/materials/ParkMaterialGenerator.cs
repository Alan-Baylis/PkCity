using UnityEngine;
using System.Collections.Generic;

namespace Pk.Materials {
    public class ParkMaterialGenerator : MaterialGenerator {
        public ParkMaterialGenerator(int seed) : base() {
            Random.InitState(seed);
            color = new Color(Random.value, Random.value, Random.value);
        }
    }
}