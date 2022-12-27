using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    public interface IReadOnlyShippable {
        Address Address { get; }
    }

    public interface IShippable : IReadOnlyShippable { 
        string Title { get; }
        string Description { get; }
    }

    public class Shippable : ScriptableObject, IShippable {
        [SerializeField]
        protected Address _address;
        public Address Address { get { return _address; } set { _address = value; } }

        [SerializeField]
        [Tooltip("The title.")]
        protected string _title;
        public string Title { get { return _title; } set { _title = value; } }

        [SerializeField]
        [Tooltip("The description.")]
        protected string _description;
        public string Description { get { return _description; } set { _description = value; } }
    }
}
