using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XianDict
{
    public class Trie<TValue>
    {
        private TrieNode root;

        public class TrieNode
        {
            public TValue val;
            internal Dictionary<char, TrieNode> next;

            public TrieNode()
            {
                val = default(TValue);
                next = new Dictionary<char, TrieNode>();
            }
        }

        public bool Contains(string key)
        {
            return !EqualityComparer<TValue>.Default.Equals(Get(key), default(TValue));
        }

        public TValue Get(string key)
        {
            TrieNode node = get(root, key, 0);
            if (node == null) return default(TValue);
            return node.val;
        }

        private TrieNode get(TrieNode node, string key, int d)
        {
            if (node == null) return null;
            if (d == key.Length) return node;
            TrieNode nextNode;
            bool hasNext = node.next.TryGetValue(key[d], out nextNode);
            return get(hasNext ? nextNode : null, key, d + 1);
        }

        public void Put(string key, TValue val)
        {
            root = put(root, key, val, 0);
        }

        private TrieNode put(TrieNode node, string key, TValue val, int d)
        {
            if (node == null) node = new TrieNode();
            if (d == key.Length)
            {
                node.val = val;
                return node;
            }
            char c = key[d];
            TrieNode nextNode;
            bool hasNext = node.next.TryGetValue(c, out nextNode);
            node.next[c] = put(hasNext ? nextNode : null, key, val, d + 1);
            return node;
        }

        public IEnumerable<TValue> GetAll(string key)
        {
            List<TValue> results = new List<TValue>();
            getAll(root, key, 0, results);
            results.Reverse();
            return results;
        }

        private void getAll(TrieNode node, string query, int d, List<TValue> q)
        {
            if (node == null) return;
            if (!EqualityComparer<TValue>.Default.Equals(node.val, default(TValue))) q.Add(node.val);
            if (d == query.Length) return;
            TrieNode nextNode;
            bool hasNext = node.next.TryGetValue(query[d], out nextNode);
            getAll(hasNext ? nextNode : null, query, d + 1, q);
        }

        public bool ContainsPrefix(string query)
        {
            TrieNode node = root;
            for (int d = 0; d < query.Length; d++)
            {
                if (!node.next.TryGetValue(query[d], out node))
                {
                    return false;
                }
                //if (!EqualityComparer<TValue>.Default.Equals(node.val, default(TValue)))
                //{
                //    return true;
                //}
            }
            return true;
        }
    }
}
