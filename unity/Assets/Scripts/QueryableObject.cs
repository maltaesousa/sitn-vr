using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SITN
{
    public class QueryableObject : MonoBehaviour
    {
        private Dictionary<string, string> data;
        public List<string[]> rawData = new List<string[]>();

        QueryableObject()
        {
            new Dictionary<string, string>();
        }

        public Dictionary<string, string> GetData()
        {
            return data;
        }

        public void SetData(Dictionary<string, string> value)
        {
            foreach (var entry in value)
            {
                rawData.Add(new string[2] { entry.Key , entry.Value });
            }
        }

        public override string ToString()
        {
            List<string> dataString = new List<string>();
            foreach (var entry in data)
            {
                dataString.Add(entry.Key + ": " + entry.Value);
            }
            return String.Join(Environment.NewLine, dataString.ToArray());
        }
    }
}
