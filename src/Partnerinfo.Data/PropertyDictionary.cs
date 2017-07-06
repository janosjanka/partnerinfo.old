// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Partnerinfo
{
    [CollectionDataContract]
    public sealed class PropertyDictionary : IDictionary<string, object>
    {
        /// <summary>
        /// Declares an internal dictionary avoiding expensive inheritance (see decorator pattern).
        /// </summary>
        private readonly Dictionary<string, object> _dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDictionary" /> class.
        /// </summary>
        public PropertyDictionary()
        {
            _dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDictionary" /> class.
        /// </summary>
        /// <param name="dictionary">A dictionary that contains Pascal case keys.</param>
        /// <exception cref="System.ArgumentNullException">dictionary</exception>
        public PropertyDictionary(IDictionary<string, object> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            _dictionary = new Dictionary<string, object>(dictionary, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="PropertyDictionary" />.
        /// </summary>
        /// <value>
        /// The number of key/value pairs contained in the <see cref="PropertyDictionary" />.
        /// </value>
        public int Count => _dictionary.Count;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly => ((IDictionary<string, object>)_dictionary).IsReadOnly;

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="PropertyDictionary" />.
        /// </summary>
        /// <value>
        /// A <see cref="PropertyDictionary.KeyCollection" /> containing the keys in the <see cref="PropertyDictionary" />.
        /// </value>
        public ICollection<string> Keys => _dictionary.Keys;

        /// <summary>
        /// Gets a collection containing the values in the <see cref="PropertyDictionary" />.
        /// </summary>
        /// <value>
        /// A <see cref="PropertyDictionary.ValueCollection" /> containing the values in the <see cref="PropertyDictionary" />.
        /// </value>
        public ICollection<object> Values => _dictionary.Values;

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object" />.
        /// </value>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key.
        /// </returns>
        public object this[string key]
        {
            get
            {
                object value;
                TryGetValue(key, out value);
                return value;
            }
            set
            {
                _dictionary[key] = value;
            }
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        public void Add(string key, object value) => _dictionary.Add(key, value);

        /// <summary>
        /// Adds the specified key and value to the dictionary if the key does not exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        public void AddIfNotExists(string key, object value)
        {
            if (!_dictionary.ContainsKey(key))
            {
                _dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="PropertyDictionary" /> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="PropertyDictionary" />.</param>
        /// <returns>
        ///   <c>true</c> if the <see cref="PropertyDictionary" /> contains an element with the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

        /// <summary>
        /// Removes the value with the specified key from the <see cref="PropertyDictionary" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        ///   <c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(string key) => _dictionary.Remove(key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified
        /// key, if the key is found; otherwise, the default value for the type of the value parameter.
        /// This parameter is passed uninitialized.</param>
        /// <returns>
        ///   <c>true</c> if the <see cref="PropertyDictionary" /> contains an element with the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue(string key, out object value) => _dictionary.TryGetValue(key, out value);

        /// <summary>
        /// Adds an item to the <see cref="PropertyDictionary" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="PropertyDictionary" />.</param>
        public void Add(KeyValuePair<string, object> item) => ((IDictionary<string, object>)_dictionary).Add(item);

        /// <summary>
        /// Removes all keys and values from the <see cref="PropertyDictionary" />.
        /// </summary>
        public void Clear() => _dictionary.Clear();

        /// <summary>
        /// Determines whether the <see cref="PropertyDictionary" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="PropertyDictionary" />.</param>
        /// <returns>
        ///   <c>true</c> if item is found in <see cref="PropertyDictionary" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(KeyValuePair<string, object> item) => ((IDictionary<string, object>)_dictionary).Contains(item);

        /// <summary>
        /// Copies the elements of the <see cref="PropertyDictionary" /> to an <see cref="System.Array" />, starting at a particular <see cref="System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array" /> that is the destination of the elements
        /// copied from <see cref="PropertyDictionary" />. The <see cref="System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => ((IDictionary<string, object>)_dictionary).CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="PropertyDictionary" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="PropertyDictionary" />.</param>
        /// <returns>
        /// <c>true</c> if item was successfully removed from the <see cref="PropertyDictionary" />;
        /// otherwise, false. This method also returns false if item is not found in the original <see cref="PropertyDictionary" />.
        /// </returns>
        public bool Remove(KeyValuePair<string, object> item) => ((IDictionary<string, object>)_dictionary).Remove(item);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="PropertyDictionary" />.
        /// </summary>
        /// <returns>
        /// A <see cref="PropertyDictionary.Enumerator" /> structure for the <see cref="PropertyDictionary" />.
        /// </returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _dictionary.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();
    }
}
