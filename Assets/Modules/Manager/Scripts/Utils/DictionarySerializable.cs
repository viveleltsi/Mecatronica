using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dan.Manager
{
    [Serializable]
    public class DictionarySerializable<T1, T2> : ISerializationCallbackReceiver 
    {
        /// <summary>
        /// All keys
        /// </summary>
        [SerializeField]
        public List<T1> Keys = new List<T1>();

        /// <summary>
        /// All values
        /// </summary>
        [SerializeField]
        public List<T2> Values = new List<T2>();

        /// <summary>
        /// Main dictionary
        /// </summary>
        public Dictionary<T1, T2> Dictionary = new Dictionary<T1, T2>();

        /// <summary>
        /// Constructor
        /// </summary>
        public DictionarySerializable(){
        }

        public void OnAfterDeserialize()
        {
            Dictionary = new Dictionary<T1, T2>();
            for (int i = 0; i < Keys.Count(); i++)
            {
                Dictionary.Add(Keys[i], Values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            Keys.Clear();
            Values.Clear();
            foreach(var valuePair in Dictionary)
            {
                Keys.Add(valuePair.Key);
                Values.Add(valuePair.Value);
            }
        }
    }
}
