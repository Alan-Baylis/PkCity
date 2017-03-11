using UnityEngine;
using System.Collections.Generic;

namespace Pk.Materials {
    public class MaterialGenerator : Material {
        static List<MaterialGenerator> generated = new List<MaterialGenerator>();

        public MaterialGenerator() : base(Resources.Load<Material>("Default")) {
            color = Color.grey;

            generated.Add(this);
        }

        static T Get<T>() where T : MaterialGenerator {
            foreach (var mat in generated)
                if (mat.GetType() == typeof(T))
                    return (T) mat;
            return null;
        }
    }
}