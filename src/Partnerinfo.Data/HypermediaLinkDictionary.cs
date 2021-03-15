// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Partnerinfo
{
    /// <summary>
    /// Represents a group of hypermedia links.
    /// http://tools.ietf.org/html/draft-kelly-json-hal-03#section-5
    /// </summary>
    [CollectionDataContract]
    public sealed class HypermediaLinkDictionary : IDictionary<string, HypermediaLink>
    {
        /// <summary>
        /// Declares an internal dictionary avoiding expensive inheritance (see decorator pattern).
        /// </summary>
        private readonly Dictionary<string, HypermediaLink> _dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="HypermediaLinkDictionary" /> class.
        /// </summary>
        public HypermediaLinkDictionary()
        {
            _dictionary = new Dictionary<string, HypermediaLink>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HypermediaLinkDictionary" /> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public HypermediaLinkDictionary(IDictionary<string, HypermediaLink> dictionary)
        {
            _dictionary = new Dictionary<string, HypermediaLink>(dictionary, StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="HypermediaLinkDictionary" />.
        /// </summary>
        /// <value>
        /// The number of key/value pairs contained in the <see cref="HypermediaLinkDictionary" />.
        /// </value>
        public int Count => _dictionary.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="System.Collections.Generic.ICollection<T>" /> is read-only.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the System.Collections.Generic.ICollection<T> is read-only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly => ((IDictionary<string, HypermediaLink>)_dictionary).IsReadOnly;

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="HypermediaLinkDictionary" />.
        /// </summary>
        /// <value>
        /// A <see cref="HypermediaLinkDictionary.KeyCollection" /> containing the keys in the <see cref="HypermediaLinkDictionary" />.
        /// </value>
        public ICollection<string> Keys => _dictionary.Keys;

        /// <summary>
        /// Gets a collection containing the values in the <see cref="HypermediaLinkDictionary" />.
        /// </summary>
        /// <value>
        /// A <see cref="HypermediaLinkDictionary.ValueCollection" /> containing the values in the <see cref="HypermediaLinkDictionary" />.
        /// </value>
        public ICollection<HypermediaLink> Values => _dictionary.Values;

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key.
        /// </returns>
        public HypermediaLink this[string key]
        {
            get
            {
                HypermediaLink value;
                TryGetValue(key, out value);
                return value;
            }
            set
            {
                _dictionary[key] = value;
            }
        }

        /// <summary>
        /// Inserts or updates the specified key and value.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="link">The link to add.</param>
        /// <exception cref="System.ArgumentNullException">link</exception>
        public void SetLink(string key, HypermediaLink link) => this[key] = link;

        /// <summary>
        /// Inserts or updates the specified key and value.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="href">A URI or a URL template.</param>
        public void SetLink(string key, string href) => this[key] = new HypermediaLink(href);

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        public void Add(string key, HypermediaLink value) => _dictionary.Add(key, value);

        /// <summary>
        /// Determines whether the <see cref="HypermediaLinkDictionary" /> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="HypermediaLinkDictionary" />.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="HypermediaLinkDictionary" /> contains an element with the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

        /// <summary>
        /// Removes the value with the specified key from the <see cref="HypermediaLinkDictionary" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        ///     <c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.
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
        ///     <c>true</c> if the <see cref="HypermediaLinkDictionary" /> contains an element with the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue(string key, out HypermediaLink value) => _dictionary.TryGetValue(key, out value);

        /// <summary>
        /// Adds an item to the <see cref="HypermediaLinkDictionary" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="HypermediaLinkDictionary" />.</param>
        public void Add(KeyValuePair<string, HypermediaLink> item) => ((IDictionary<string, HypermediaLink>)_dictionary).Add(item);

        /// <summary>
        /// Removes all keys and values from the <see cref="HypermediaLinkDictionary" />.
        /// </summary>
        public void Clear() => _dictionary.Clear();

        /// <summary>
        /// Determines whether the <see cref="HypermediaLinkDictionary" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="HypermediaLinkDictionary" />.</param>
        /// <returns>
        ///     <c>true</c> if item is found in <see cref="HypermediaLinkDictionary" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(KeyValuePair<string, HypermediaLink> item) => ((IDictionary<string, HypermediaLink>)_dictionary).Contains(item);

        /// <summary>
        /// Copies the elements of the <see cref="HypermediaLinkDictionary" /> to an <see cref="System.Array" />, starting at a particular <see cref="System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array" /> that is the destination of the elements
        /// copied from <see cref="HypermediaLinkDictionary" />. The <see cref="System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<string, HypermediaLink>[] array, int arrayIndex) => ((IDictionary<string, HypermediaLink>)_dictionary).CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="HypermediaLinkDictionary" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="HypermediaLinkDictionary" />.</param>
        /// <returns>
        ///     <c>true</c> if item was successfully removed from the <see cref="HypermediaLinkDictionary" />;
        ///     otherwise, false. This method also returns false if item is not found in the original <see cref="HypermediaLinkDictionary" />.
        /// </returns>
        public bool Remove(KeyValuePair<string, HypermediaLink> item) => ((IDictionary<string, HypermediaLink>)_dictionary).Remove(item);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="HypermediaLinkDictionary" />.
        /// </summary>
        /// <returns>
        /// A <see cref="HypermediaLinkDictionary.Enumerator" /> structure for the <see cref="HypermediaLinkDictionary" />.
        /// </returns>
        public IEnumerator<KeyValuePair<string, HypermediaLink>> GetEnumerator() => _dictionary.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();
    }
}
