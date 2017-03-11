using UnityEngine;
using System.Collections.Generic;

namespace Pk.Generators {
    public abstract class Generator : MonoBehaviour {
        [HideInInspector]
        public Generator parent;
        [HideInInspector]
        public List<Generator> children = new List<Generator>();
        [HideInInspector]
        public List<GameObject> props = new List<GameObject>();

        public virtual void Initialize() {
            StaticBatchingUtility.Combine(gameObject);
        }

        public T Generate<T>(Vector3 localPos) where T : Generator {
            return Generate<T>(localPos, Quaternion.Euler(0, 0, 0));
        }

        public T Generate<T>(Vector3 localPos, Quaternion localRot) where T : Generator {
            var obj = new GameObject();
            obj.transform.parent = transform;

            obj.transform.localPosition = localPos;
            obj.transform.localRotation = localRot;
            obj.name = "gen_" + typeof(T).Name;

            var gen = obj.AddComponent<T>();
            gen.parent = this;

            children.Add(gen);
            return gen;
        }
        
        public GameObject CreateProp(string name, Vector3 localPos, Quaternion localRot, Mesh mesh = null, Material mat = null) {
            var prop = new GameObject("prop_" + name);
            prop.transform.parent = transform;

            prop.transform.localPosition = localPos;
            prop.transform.localRotation = localRot;

            if (mesh != null) {
                // Finalize mesh
                mesh.Optimize();
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                var renderer = prop.AddComponent<MeshRenderer>();
                    if (mat != null) renderer.material = mat;
                prop.AddComponent<MeshFilter>().mesh = mesh;
                var collider = prop.AddComponent<MeshCollider>();
                collider.sharedMesh = null;
                collider.sharedMesh = mesh;
            }

            props.Add(prop);
            return prop;
        }

        public override bool Equals(object o) {
            return o != null && GetType() == o.GetType() && seed == (o as Region).seed;
        }

        public override int GetHashCode() {
            return seed;
        }

        public int seed {
            get { return RootToSeed(transform.position); }
        }

        public static int RootToSeed(Vector3 root) {
            return root.ToString().GetHashCode();
        }
    }
}
